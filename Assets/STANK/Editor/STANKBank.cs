using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UIElements;
public class STANKBank : EditorWindow
{
    // STANKBank is the Editor Window for managing your STANK assets.
    // From the STANKBank window, you can create, edit, and delete STANKs.

    // The Vault is the singleton instance of STANKBank.
    public static STANKBank Vault;

    // Elements of the UIDocument
    VisualElement rootAsset;
    VisualTreeAsset odorListItem;
    VisualElement odorListPanel;
    StyleSheet odorWidgetSS;
    ListView odorListView;
    private VisualElement odorDetailPane;
    Label odorNameField;
    VisualElement odorHudSpriteField;
    //UnityEngine.UIElements.Image odorHudSpriteField;
    ObjectField spriteImage;
    TextField odorDescriptionField;
    FloatField partsPerMillionField;
    Label odorNameLabel;
    Label odorDescriptionLabel;
    Label ppmppLabel;
    Label hudIconLabel;
    Label gizmoColorLabel;
    VisualTreeAsset odorDetailsAsset;
    ListView odorListPane;
    ColorField gizmoColorField;
    StyleSheet odorDetailsSS;
    Texture2D defaultImageGridTexture;
    Button selectImageButton;
    SerializedProperty spriteProperty;
    SerializedObject serializedSelectedOdor;
    SerializedProperty nameProperty;
    SerializedProperty descriptionProperty;
    SerializedProperty ppmProperty;
    SerializedProperty gizmoColorProperty;
    Stank selectedOdor;
    TwoPaneSplitView splitView;
    TwoPaneSplitView splitListView;
    List<Stank> allOdors = new List<Stank>();
    Button addNewOdorButton;
    Button deleteOdorButton;

    private void OnEnable()
    {
        if (Vault == null) Vault = this;
        rootAsset = rootVisualElement;
        defaultImageGridTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/STANK/Editor/Textures/defaultimagegrid.png");
        odorDetailsSS = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/STANK/Editor/OdorHUDIcon.uss");
        odorDetailsAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/STANK/Editor/OdorHUDIcon.uxml");
        splitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);
        splitListView = new TwoPaneSplitView(0, 50, TwoPaneSplitViewOrientation.Vertical);
        odorListPanel = rootVisualElement.Q<ListView>("OdorList");
        odorListView = rootVisualElement.Q<ListView>("OdorListView");
        odorNameField = new Label();
        spriteImage = new ObjectField();
        gizmoColorField = new ColorField();
        gizmoColorLabel = new Label();
        spriteImage.objectType = typeof(Texture2D);
        odorDescriptionField = new TextField();
        partsPerMillionField = new FloatField();
        odorNameLabel = new Label();
        selectImageButton = new Button();
        odorDescriptionLabel = new Label();
        ppmppLabel = new Label();
        hudIconLabel = new Label();
        spriteProperty = null;
        odorDetailPane = new VisualElement();
        addNewOdorButton = new Button();
        deleteOdorButton = new Button();
        addNewOdorButton.text = "Create New STANK";
        addNewOdorButton.clicked += CreateNewOdor;
        odorListPane = new ListView();
        rootVisualElement.Add(new Label("STANKs"));
        rootVisualElement.styleSheets.Add(odorDetailsSS);
        rootVisualElement.Add(splitView);
        splitListView.Add(addNewOdorButton);
        splitListView.Add(odorListPane);
        splitView.Add(splitListView);
    }

    [MenuItem("STANK/STANKBank")]
    public static void ShowWindow()
    {
        STANKBank wnd = GetWindow<STANKBank>();
        wnd.titleContent = new GUIContent("STANKBank");
        wnd.Focus();
        Rect windowRect = new Rect(500f, 500f, 300f, 450f);
        wnd.position = windowRect;
        wnd.minSize = new Vector2(250f, 250f);
        
    }

    private void GenerateListView()
    {
        // Create a new item for the list view
        Func<VisualElement> makeItem = () => odorListItem.CloneTree();

        Action<VisualElement, int> bindItem = (e, i) =>
        {
            // If the name is not empty, assign the name to the label
            // If the name is empty, delete the asset at that path
            if (allOdors[i].Name != "")
                e.Q<Label>("OdorName").text = allOdors[i].Name;
            else
                AssetDatabase.DeleteAsset(allOdors[i].Name);
        };

        rootAsset.Add(odorListView);
    }

    public void RefreshListView()
    {
        // Clear the list view and rebuild it
        CreateListView();

        if (odorListPane == null) Debug.Log("odorListView not found");

        var allOdorGuids = AssetDatabase.FindAssets("t:Stank");
        allOdors.Clear();
        foreach (var guid in allOdorGuids)
        {
            allOdors.Add(AssetDatabase.LoadAssetAtPath<Stank>(AssetDatabase.GUIDToAssetPath(guid)));
        }
        odorListPane.Rebuild();
    }

    private void CreateListView()
    {
        // Initialize the list view with all sprites' names
        odorListPane.Clear();        
        odorListPane.makeItem = () => new Label();
        odorListPane.bindItem = (item, index) => { (item as Label).text = allOdors[index].Name; };
        odorListPane.itemsSource = allOdors;
    }

    public void CreateGUI()
    {
        RefreshListView();
        odorDetailPane.Add(odorDetailsAsset.Instantiate());
        hudIconLabel.text = "HUD Icon";
        //odorDetailPane.Add(hudIconLabel);
        spriteImage.label = "HUD Icon";
        odorDetailPane.Add(odorHudSpriteField);
        odorDetailPane.Add(spriteImage);
        odorNameField.text = "STANK Name";
        odorDetailPane.Add(odorNameField);
        odorDescriptionField.label = "STANK Description";
        odorDetailPane.Add(odorDescriptionField);
        partsPerMillionField.label = "Particles Per Million";
        odorDetailPane.Add(partsPerMillionField);
        gizmoColorField.label = "Gizmo Color";
        odorDetailPane.Add(gizmoColorField);
        odorHudSpriteField = odorDetailPane.Q<VisualElement>("HUDIcon");
        deleteOdorButton = new Button();
        deleteOdorButton.text = "DELETE STANK";
        deleteOdorButton.clicked += DeleteOdor;
        odorDetailPane.Add(deleteOdorButton);
        splitView.Add(odorDetailPane);
        UpdateHUDImagePreview();
        odorListPane.selectionChanged += OnOdorSelectionChange;
        odorListPane.AddToSelection(0);
        spriteImage.RegisterValueChangedCallback(x => UpdateHUDImagePreview());
    }

    private void CreateNewOdor()
    {
        CreateOdorWindow wnd = GetWindow<CreateOdorWindow>();
        wnd.titleContent = new GUIContent("Create New STANK");
    }

    private void UpdateHUDImagePreview()
    {
        // If the sprite is not null, set the background image to the sprite
        // If the sprite is null, populate it with our default grid image
        if (spriteImage.value != null)
        {
            odorHudSpriteField.style.backgroundImage = spriteProperty.objectReferenceValue as Texture2D;
        }
        else
        {
            odorHudSpriteField.style.backgroundImage = defaultImageGridTexture;
        }
    }

    private void DeleteOdor()
    {
        // Delete odor from the list and asset database
        allOdors.Remove(selectedOdor);
        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(selectedOdor));
        RefreshListView();
    }

    private void OnOdorSelectionChange(IEnumerable<object> selectedItems)
    {
        // Update the window when we change STANK selections
        selectedOdor = selectedItems.First() as Stank;
        serializedSelectedOdor = new SerializedObject(selectedItems.First() as UnityEngine.Object);
        if (serializedSelectedOdor == null)
        {
            Debug.Log(serializedSelectedOdor.ToString());
            return;
        }

        spriteProperty = serializedSelectedOdor.FindProperty("Icon");
        nameProperty = serializedSelectedOdor.FindProperty("Name");
        descriptionProperty = serializedSelectedOdor.FindProperty("Description");
        ppmProperty = serializedSelectedOdor.FindProperty("ParticlesPerMillion");
        gizmoColorProperty = serializedSelectedOdor.FindProperty("GizmoColor");
        spriteImage.BindProperty(spriteProperty);
        UpdateHUDImagePreview();
        odorNameField.BindProperty(nameProperty);
        odorDescriptionField.BindProperty(descriptionProperty);
        gizmoColorField.BindProperty(gizmoColorProperty);
    }
}
