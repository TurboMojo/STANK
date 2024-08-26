using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace STANK {
// public delegate void OnDestroyDelegate(EditorWindow window);
    public class CreateOdorWindow : EditorWindow
    {
        // Editor window for creating a new STANK

        SerializedProperty spriteProperty;
        SerializedObject selectedOdor;
        SerializedProperty nameProperty;
        SerializedProperty descriptionProperty;
        SerializedProperty particleLifetimeProperty;
        SerializedProperty particleSpeedProperty;
        SerializedProperty pmppProperty;
        SerializedProperty scentMemoryProperty;
        StyleSheet odorDetailsSS;
        Texture2D defaultImageGridTexture;
        Button saveOdorButton;
        Button createOdorButton;

        TextField odorNameField;
        VisualElement odorHudSpriteField;
        ObjectField spriteImage;
        TextField odorDescriptionField;
        FloatField partsPerMillionPerParticleField;
        FloatField odorParticleLifetime;
        FloatField odorParticleSpeed;
        CurveField scentMemoryCurve;
        Label odorNameLabel;
        Label odorDescriptionLabel;
        Label ppmppLabel;
        Label odorPLLabel;
        Label odorPSLabel;
        Label odorScentMemoryLabel;
        Label hudIconLabel;
        Stank newOdor;
        private VisualElement odorDetailPane;
        VisualTreeAsset odorDetailsAsset;
        CreateOdorWindow wnd;

        [MenuItem("Tools/STANK/Create Odor")]
        public static void ShowWindow()
        {
            CreateOdorWindow wnd = GetWindow<CreateOdorWindow>();
            wnd.titleContent = new GUIContent("Create New STANK");
        }
        
        // Start is called before the first frame update
        void OnEnable()
        {
            odorNameField = new TextField();
            odorDescriptionField = new TextField();
            partsPerMillionPerParticleField = new FloatField();
            odorParticleLifetime = new FloatField();
            odorParticleSpeed = new FloatField();
            scentMemoryCurve = new CurveField();
            odorNameLabel = new Label();
            odorDescriptionLabel = new Label();
            ppmppLabel = new Label();
            odorPLLabel = new Label();
            odorPSLabel = new Label();
            hudIconLabel = new Label();
            spriteImage = new ObjectField();
            newOdor = ScriptableObject.CreateInstance("Stank") as Stank;
            defaultImageGridTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/STANK/Editor/Textures/defaultimagegrid.png");
            odorDetailsSS = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/STANK/Editor/OdorHUDIcon.uss");
            odorDetailsAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/STANK/Editor/OdorHUDIcon.uxml");
        }

        public void CreateGUI()
        {
            odorDetailPane = new VisualElement();
            spriteImage.objectType = typeof(Texture2D);
            rootVisualElement.styleSheets.Add(odorDetailsSS);
            odorDetailPane.Add(odorDetailsAsset.Instantiate());
            hudIconLabel.text = "HUD Icon";
            //odorDetailPane.Add(hudIconLabel);
            spriteImage.label = "HUD Icon";
            odorDetailPane.Add(spriteImage);
            odorDetailPane.Add(odorHudSpriteField);
            odorNameField.label = "Odor Name";
            odorDetailPane.Add(odorNameField);
            odorDescriptionField.label = "Odor Description";
            odorDetailPane.Add(odorDescriptionField);
            partsPerMillionPerParticleField.label = "PPM Per Particle";
            odorDetailPane.Add(partsPerMillionPerParticleField);
            odorParticleLifetime.label = "Particle Lifetime";
            odorDetailPane.Add(odorParticleLifetime);
            odorParticleSpeed.label = "Particle Speed";
            odorDetailPane.Add(odorParticleSpeed);
            scentMemoryCurve.label = "Scent Memory";
            odorDetailPane.Add(scentMemoryCurve);
            rootVisualElement.Add(odorDetailPane);
            odorHudSpriteField = odorDetailPane.Q<VisualElement>("HUDIcon");
            createOdorButton = new Button();
            createOdorButton.text = "Create Odor";
            createOdorButton.clicked += SaveOdor;
            odorDetailPane.Add(createOdorButton);
            
            CreateNewOdor();
        }

        private void SaveOdor()
        {
            Debug.Log("SaveOdor");
            AssetDatabase.CreateAsset(newOdor, "Assets/STANK/SOStank/Odors/Chemicals/" + nameProperty.stringValue + ".asset");
            AssetDatabase.SaveAssets();
            STANKBank.Vault.RefreshSTANKListView();
            CreateNewOdor();
        }


        private void CreateNewOdor()
        {        
            newOdor = ScriptableObject.CreateInstance("Stank") as Stank; 
            selectedOdor = new SerializedObject(newOdor);
            spriteProperty = selectedOdor.FindProperty("Icon");
            nameProperty = selectedOdor.FindProperty("Name");
            descriptionProperty = selectedOdor.FindProperty("Description");
            particleLifetimeProperty = selectedOdor.FindProperty("ParticleLifetime");
            particleSpeedProperty = selectedOdor.FindProperty("ParticleSpeed");
            pmppProperty = selectedOdor.FindProperty("PPMPP");
            scentMemoryProperty = selectedOdor.FindProperty("ScentMemory");
            spriteImage.BindProperty(spriteProperty);        
            odorNameField.BindProperty(nameProperty);
            odorDescriptionField.BindProperty(descriptionProperty);
            partsPerMillionPerParticleField.BindProperty(pmppProperty);
            odorParticleLifetime.BindProperty(particleLifetimeProperty);
            odorParticleSpeed.BindProperty(particleSpeedProperty);
            scentMemoryCurve.BindProperty(scentMemoryProperty);
            UpdateHUDImagePreview();
            STANKBank.Vault.RefreshSTANKListView();
        }

        private void UpdateHUDImagePreview()
        {
            if (spriteImage.value != null)
            {
                odorHudSpriteField.style.backgroundImage = spriteProperty.objectReferenceValue as Texture2D;
            }
            else
            {
                odorHudSpriteField.style.backgroundImage = defaultImageGridTexture;
            }
        }
    }
}