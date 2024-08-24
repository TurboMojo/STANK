using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UIElements;

namespace STANK {
    public class ImprovedSTANKBank : EditorWindow
    {
        public static ImprovedSTANKBank Vault;

        // Elements of the UIDocument
        VisualElement rootAsset;
        Texture2D defaultImageGridTexture;

        VisualElement stankBankDetailsWindow;
        UIDocument stankBankAsset;        

        // STANKS tab elements
        VisualTreeAsset stankDetailsAsset;
        VisualTreeAsset stankListItem;
        VisualElement stankBankWindow_STANKS;
        VisualElement stankBankWindow_STANKResponses;
        VisualElement stankBankWindow_Smellers;
        VisualElement stankBankWindow_Fellers;
        
        ListView stankListView;
        List<Stank> allSTANKs = new List<Stank>();
        List<STANKResponse> allSTANKResponses = new List<STANKResponse>();
        ObjectField iconField;
        VisualElement stankHudSpriteField;
        VisualElement stankDetailsModule;
        SerializedProperty spriteProperty;
        Button deleteSTANKButton;
        Stank selectedSTANK;
        SerializedObject serializedSelectedSTANK;

        STANKResponse selectedSTANKResponse;
        SerializedObject serializedSelectedSTANKResponse;
        SerializedProperty nameProperty;
        SerializedProperty descriptionProperty;    
        SerializedProperty gizmoColorProperty;
        TextField stankNameField;
        TextField stankDescriptionField;    
        ColorField gizmoColorField;
        ToolbarButton stanksButton;
        ToolbarButton stankResponsesButton;
        ToolbarButton smellersButton;
        ToolbarButton fellersButton;
        ObjectField hudMaterialField;
        
        // STANKReactions tab elements
        ObjectField responseStankField;
        FloatField pungencyThresholdField;
        FloatField responseDelayField;
        ObjectField AnimationClipField;
        Button deleteResponsesButton;

        SerializedProperty responseStankProperty;
        SerializedProperty pungencyThresholdProperty;
        SerializedProperty responseDelayProperty;
        SerializedProperty AnimationClipProperty;
        

        private void OnEnable(){
            if (Vault == null) Vault = this; else GameObject.Destroy(this);
            
            defaultImageGridTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/STANK/Editor/Textures/defaultimagegrid.png");
            Debug.Log("defaultImageGridTexture: "+defaultImageGridTexture);

            BuildBANKWindow();
            BuildToolBar();
            BuildSTANKSTab();
            BuildSTANKResponsesTab();
            ShowSTANKSTab();
            //if(deleteSTANKButton == null) Debug.Log("deleteSTANKButton not found");
            //deleteSTANKButton.clicked += DeleteSTANK;
            spriteProperty = null;
        }

        void BuildToolBar(){
            Debug.Log("building toolbar");
            stanksButton = rootAsset.Q<ToolbarButton>("STANKsToolbarButton");            
            stankResponsesButton = rootAsset.Q<ToolbarButton>("STANKReactionsToolbarButton");            
            smellersButton = rootAsset.Q<ToolbarButton>("SmellersToolbarButton");            
            fellersButton = rootAsset.Q<ToolbarButton>("FellersToolbarButton");            

            stanksButton.clicked += ShowSTANKSTab;
            stankResponsesButton.clicked += ShowSTANKResponsesTab;
            smellersButton.clicked += ShowSmellersTab;
            fellersButton.clicked += ShowFellersTab;
        }

        void BuildBANKWindow(){
            VisualTreeAsset stankBankDocument = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/STANK/Editor/STANKBank/ImprovedSTANKBank.uxml");
            // Load the elements of the UIDocument
            rootAsset = stankBankDocument.CloneTree();
            if(rootAsset == null) Debug.Log("rootAsset not found");
            rootVisualElement.Add(rootAsset);
            stankBankDetailsWindow = rootAsset.Q<VisualElement>("STANKBankMainPanel");
            stankBankWindow_STANKS = rootAsset.Q<VisualElement>("STANKsDetailsWindow");
            stankBankDetailsWindow.Add(stankBankWindow_STANKS);
            stankBankDetailsWindow.Add(stankBankWindow_STANKResponses);
            //stankBankDetailsWindow.Add(stankBankWindow_Smellers);
            //stankBankDetailsWindow.Add(stankBankWindow_Fellers);
        }

        void BuildSTANKSTab(){
            if(stankBankWindow_STANKS == null) Debug.Log("stankBankWindow_STANKS not found");
            stankListView = stankBankWindow_STANKS.Q<ListView>("STANKSListView");
            stankBankWindow_STANKS = stankBankDetailsWindow.Q<VisualElement>("STANKsDetailsWindow");
            stankBankWindow_STANKResponses = stankBankDetailsWindow.Q<VisualElement>("STANKResponsesDetailsWindow");
            if(stankBankWindow_STANKS == null) Debug.Log("stankBankWindow_STANKS not found");
            stankDetailsModule = stankBankWindow_STANKS.Q<VisualElement>("STANKsDetailsWindow");
            if(stankDetailsModule == null) Debug.Log("stankDetailsModule not found");
            iconField = stankDetailsModule.Q<ObjectField>("IconField");
            if(iconField == null) Debug.Log("IconField not found");
            stankHudSpriteField = stankDetailsModule.Q<ObjectField>("IconField");
            deleteSTANKButton = stankDetailsModule.Q<Button>("DeleteSTANKButton");
            //VisualElement stanksDetailsPanel = stankDetailsModule.Q<VisualElement>("STANKDetailsPanel");
            //if(stanksDetailsPanel == null) Debug.Log("stanksDetailsPanel not found");
            //stanksDetailsPanel.Add(stankDetailsModule);
            stankNameField = stankDetailsModule.Q<TextField>("NameField");
            stankDescriptionField = stankDetailsModule.Q<TextField>("DescriptionField");
            gizmoColorField = stankDetailsModule.Q<ColorField>("GizmoColorField");
            hudMaterialField = stankDetailsModule.Q<ObjectField>("HUDMaterialField");
        }

        void BuildSTANKResponsesTab(){
            
        }

        void BuildFellersTab(){
            
        }

        void BuildSmellersTab(){
            
        }

        void ShowSTANKSTab(){
            stankBankWindow_STANKS.style.display = DisplayStyle.Flex;
            stankBankWindow_STANKResponses.style.display = DisplayStyle.None;
            //stankBankWindow_Fellers.style.display = DisplayStyle.None;
            //stankBankWindow_Smellers.style.display = DisplayStyle.None;
            stankBankWindow_STANKS.BringToFront();
        }
        void ShowSTANKResponsesTab(){
            stankBankWindow_STANKS.style.display = DisplayStyle.None;
            stankBankWindow_STANKResponses.style.display = DisplayStyle.Flex;
            //stankBankWindow_Fellers.style.display = DisplayStyle.None;
            //stankBankWindow_Smellers.style.display = DisplayStyle.None;
            stankBankWindow_STANKResponses.BringToFront();
        }
        void ShowSmellersTab(){
            
        }

        void ShowFellersTab(){
            
        }

        public void CreateGUI()
        {
            if(rootAsset == null) Debug.Log("rootAsset not found");
            stankListView = rootAsset.Q<ListView>("STANKSListView");
            
            RefreshSTANKListView();
            if(stankListView != null) stankListView.AddToSelection(0);
            else Debug.Log("stankListView is null");
            //RefreshSTANKResponseListView();
            stankHudSpriteField = rootAsset.Q<VisualElement>("IconPreview");
            deleteSTANKButton.clicked += DeleteSTANK;
            
            UpdateHUDImagePreview();
        }       

        private void OnSTANKSelectionChange(IEnumerable<object> selectedItems)
        {
            
            // Update the window when we change STANK selections
            selectedSTANK = selectedItems.First() as Stank;
            serializedSelectedSTANK = new SerializedObject(selectedSTANK as UnityEngine.Object);
            if (serializedSelectedSTANK == null)
            {
                Debug.Log("serializedSelectedSTANK not found");
                return;
            } else {
                Debug.Log(serializedSelectedSTANK.ToString());
            }

            spriteProperty = serializedSelectedSTANK.FindProperty("Icon");
            nameProperty = serializedSelectedSTANK.FindProperty("Name");
            descriptionProperty = serializedSelectedSTANK.FindProperty("Description");            
            gizmoColorProperty = serializedSelectedSTANK.FindProperty("GizmoColor");

            if(iconField != null && spriteProperty != null) iconField.BindProperty(spriteProperty);
            else Debug.Log(("spriteProperty not found"));
            //UpdateHUDImagePreview();
            if(nameProperty != null) stankNameField.BindProperty(nameProperty);
            else Debug.Log(("nameProperty not found"));
            if(descriptionProperty != null) stankDescriptionField.BindProperty(descriptionProperty);
            else Debug.Log(("descriptionProperty not found"));
            if(gizmoColorProperty != null) gizmoColorField.BindProperty(gizmoColorProperty);
            else Debug.Log(("gizmoColorProperty not found"));
            
        } 

        private void OnSTANKResponseSelectionChange(IEnumerable<object> selectedItems)
        {            
            // Update the window when we change STANK selections
            selectedSTANKResponse = selectedItems.First() as STANKResponse;
            serializedSelectedSTANKResponse = new SerializedObject(selectedItems.First() as UnityEngine.Object);
            if (serializedSelectedSTANKResponse == null)
            {                
                return;
            }

            responseStankProperty = serializedSelectedSTANKResponse.FindProperty("Stank");
            pungencyThresholdProperty = serializedSelectedSTANKResponse.FindProperty("PungencyThreshold");
            responseDelayProperty = serializedSelectedSTANKResponse.FindProperty("ResponseDelay");
            AnimationClipProperty = serializedSelectedSTANKResponse.FindProperty("AnimationClip");

            responseStankField.BindProperty(responseStankProperty);
            pungencyThresholdField.BindProperty(pungencyThresholdProperty);
            responseDelayField.BindProperty(responseDelayProperty);
            AnimationClipField.BindProperty(AnimationClipProperty);            
        }
        
        private void DeleteSTANK(){
            // Delete odor from the list and asset database
            allSTANKs.Remove(selectedSTANK);
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(selectedSTANK));
            RefreshSTANKListView();
        }

        private void DeleteSTANKResponse(){
            // Delete odor from the list and asset database
            allSTANKResponses.Remove(selectedSTANKResponse);
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(selectedSTANKResponse));
            RefreshSTANKResponseListView();
        }

        [MenuItem("Tools/STANK/ImprovedSTANKBank")]
        public static void ShowWindow()
        {
            ImprovedSTANKBank wnd = GetWindow<ImprovedSTANKBank>();
            wnd.titleContent = new GUIContent("STANKBank");
            wnd.Focus();
            Rect windowRect = new Rect(500f, 500f, 300f, 450f);
            wnd.position = windowRect;
            wnd.minSize = new Vector2(250f, 250f);            
        }

        private void CreateSTANKListView()
        {
            // Initialize the list view with all sprites' names
            stankListView.Clear();        
            stankListView.makeItem = () => new Label();
            stankListView.bindItem = (item, index) => { (item as Label).text = allSTANKs[index].Name; };
            stankListView.itemsSource = allSTANKs;
        }

        public void RefreshSTANKListView()
        {
            // Clear the list view and rebuild it
            CreateSTANKListView();

            if (stankListView == null) Debug.Log("stankListView not found");

            var allSTANKGuids = AssetDatabase.FindAssets("t:Stank");
            allSTANKs.Clear();
            foreach (var guid in allSTANKGuids)
            {
                allSTANKs.Add(AssetDatabase.LoadAssetAtPath<Stank>(AssetDatabase.GUIDToAssetPath(guid)));
            }
            
            stankListView.Rebuild();
            stankListView.selectionChanged += OnSTANKSelectionChange;
        }

        public void RefreshSTANKResponseListView()
        {
            // Clear the list view and rebuild it
            CreateSTANKResponseListView();

            if (stankListView == null) Debug.Log("stankListView not found");

            var allOdorGuids = AssetDatabase.FindAssets("t:STANKResponse");
            allSTANKResponses.Clear();
            foreach (var guid in allOdorGuids)
            {
                allSTANKResponses.Add(AssetDatabase.LoadAssetAtPath<STANKResponse>(AssetDatabase.GUIDToAssetPath(guid)));
            }
            stankListView.Rebuild();
        }



        private void CreateSTANKResponseListView()
        {
            // Initialize the list view with all sprites' names
            stankListView.Clear();        
            stankListView.makeItem = () => new Label();
            stankListView.bindItem = (item, index) => { (item as Label).text = allSTANKResponses[index].name; };
            stankListView.itemsSource = allSTANKResponses;
        }

        private void UpdateHUDImagePreview()
        {
            // If the sprite is not null, set the background image to the sprite
            // If the sprite is null, populate it with our default grid image
            if(iconField == null) { Debug.Log("spriteImage not found"); return;}
            if(stankHudSpriteField == null) { Debug.Log("stankHudSpriteField not found"); return;}
            if (iconField.value != null)
            {
                stankHudSpriteField.style.backgroundImage = spriteProperty.objectReferenceValue as Texture2D;
            }
            else
            {
                if(defaultImageGridTexture == null) {Debug.Log("defaultImageGridTexture not found"); return;}
                stankHudSpriteField.style.backgroundImage = defaultImageGridTexture;
            }
            iconField.RegisterValueChangedCallback(x => UpdateHUDImagePreview());
        }
    }
}