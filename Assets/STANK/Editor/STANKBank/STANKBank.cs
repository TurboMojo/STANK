using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UIElements;


namespace STANK {
    public class STANKBank : EditorWindow
    {
        public static STANKBank Vault;

        
        private const string currentlySelectedTabClassName = "selected";
        private const string hiddenClassName = "hidden";

        // Elements of the UIDocument
        VisualElement rootAsset;
        Texture2D defaultImageGridTexture;       
        UIDocument stankBankAsset;    
        List<Stank> allSTANKs = new List<Stank>();
        List<STANKResponse> allSTANKResponses = new List<STANKResponse>();    
        List<Smeller> allSmellers = new List<Smeller>();
        List<Feller> allFellers = new List<Feller>();
        string stankSavePath = "Assets/STANK/SOStank/Stanks/";
        string responseSavePath = "Assets/STANK/SOStank/Responses/";

        // Toolbar
        ToolbarButton stanksButton;
        ToolbarButton stankResponsesButton;
        ToolbarButton smellersButton;
        ToolbarButton fellersButton;
        
        // Content panels
        ListView stankListView;
        VisualElement stankBankTab_STANKS;
        VisualElement stankBankTab_STANKResponses;
        VisualElement stankBankTab_Smellers;
        VisualElement stankBankTab_Fellers;

        // STANKS tab elements        
        ObjectField iconField;
        VisualElement stankHudSpriteField;
        TextField stankNameField;
        TextField stankDescriptionField;
        Button createSTANKButton;
        Button saveSTANKButton;
        Button deleteSTANKButton;
        Stank selectedSTANK;
        SerializedObject serializedSelectedSTANK;
        SerializedProperty iconProperty;
        SerializedProperty stankNameProperty;
        SerializedProperty stankDescriptionProperty;
        SerializedProperty descriptionProperty;    
        SerializedProperty gizmoColorProperty;
        bool isNewSTANK = false;

        // STANKResponse tab elements
        ListView stankResponseListView;
        STANKResponse selectedSTANKResponse;
        SerializedObject serializedSelectedSTANKResponse;
        SerializedProperty stankResponseNameProperty;
        TextField stankResponseNameField;
        TextField stankResponseDescriptionField;    
        ColorField gizmoColorField;
        ObjectField hudMaterialField;
        STANKResponse newStankResponse;
        Button deleteSTANKResponseButton;
        Button createSTANKResponseButton;
        Button saveSTANKResponseButton;
        ObjectField responseStankField;
        FloatField pungencyThresholdField;
        FloatField responseDelayField;
        ObjectField AnimationClipField;
        Button deleteResponseButton;
        bool isNewSTANKResponse = false;

        SerializedProperty serializedResponseNameProperty;
        SerializedProperty responseStankProperty;
        SerializedProperty pungencyThresholdProperty;
        SerializedProperty responseDelayProperty;
        SerializedProperty AnimationClipProperty;
        
        // Smellers tab elements
        ListView smellersListView;
        ObjectField smellerStankField;
        FloatField smellerRadiusField;
        CurveField pungencyCurveField;
        FloatField expansionRateField;
        Toggle showStankLinesToggle;
        PropertyField stankLinesEmittersField;

        // Fellers tab elements
        ListView fellersListView;


        private void OnEnable(){
            if (Vault == null) Vault = this; else GameObject.Destroy(this);
            
            defaultImageGridTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/STANK/Editor/Textures/defaultimagegrid.png");
            
            BuildBANKWindow();
            BuildToolBar();
            BuildSTANKSTab();
            BuildSTANKResponsesTab();
            iconProperty = null;
        }

        void BuildToolBar(){
            
            stanksButton = rootAsset.Q<ToolbarButton>("STANKsToolbarButton");            
            stankResponsesButton = rootAsset.Q<ToolbarButton>("STANKResponsesToolbarButton");            
            smellersButton = rootAsset.Q<ToolbarButton>("SmellersToolbarButton");            
            fellersButton = rootAsset.Q<ToolbarButton>("FellersToolbarButton");            

            stanksButton.clicked += ShowSTANKSTab;
            stankResponsesButton.clicked += ShowSTANKResponsesTab;
        }

        void BuildBANKWindow(){
            VisualTreeAsset stankBankDocument = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/STANK/Editor/STANKBank/STANKBank.uxml");
            // Load the elements of the UIDocument
            rootAsset = stankBankDocument.CloneTree();
            if(rootAsset == null) Debug.Log("rootAsset not found");
            rootVisualElement.Add(rootAsset);            
            stankBankTab_STANKS = rootAsset.Q<VisualElement>("STANKsTab");  
            if(stankBankTab_STANKS == null) Debug.Log("stankBankWindow_STANKS not found");          
            stankBankTab_STANKResponses = rootAsset.Q<VisualElement>("STANKResponsesTab");
        }

        void BuildSTANKSTab(){
            if(stankBankTab_STANKS == null) Debug.Log("stankBankWindow_STANKS not found");
            stankListView = rootAsset.Q<ListView>("STANKSListView");
            stankListView.selectionChanged += OnSTANKSelectionChange;
            stankBankTab_STANKS = rootAsset.Q<VisualElement>("STANKsTab");            
            if(stankBankTab_STANKS == null) Debug.Log("stankBankWindow_STANKS not found");            
            iconField = stankBankTab_STANKS.Q<ObjectField>("IconField");
            if(iconField == null) Debug.Log("IconField not found");
            iconField.RegisterValueChangedCallback(x => UpdateHUDImagePreview());
            stankHudSpriteField = stankBankTab_STANKS.Q<ObjectField>("IconField");
            stankNameField = stankBankTab_STANKS.Q<TextField>("NameField");
            stankDescriptionField = stankBankTab_STANKS.Q<TextField>("DescriptionField");
            gizmoColorField = stankBankTab_STANKS.Q<ColorField>("GizmoColorField");
            hudMaterialField = stankBankTab_STANKS.Q<ObjectField>("HUDMaterialField");            
            stankHudSpriteField = rootAsset.Q<VisualElement>("IconPreview");
            deleteSTANKButton = stankBankTab_STANKS.Q<Button>("DeleteSTANKButton");
            deleteSTANKButton.clicked += DeleteSTANK;
            createSTANKButton = stankBankTab_STANKS.Q<Button>("CreateSTANKButton");
            createSTANKButton.clicked += CreateSTANK;
            saveSTANKButton = stankBankTab_STANKS.Q<Button>("SaveSTANKButton");
            saveSTANKButton.clicked += SaveSTANK;
        }

        void SaveSTANK(){
            // Toggle the Delete STANK button on
            deleteSTANKButton.ToggleInClassList(hiddenClassName);
            // Toggle the Save STANK button off
            saveSTANKButton.ToggleInClassList(hiddenClassName);

            // Store asset
            AssetDatabase.CreateAsset(selectedSTANK, stankSavePath + stankNameProperty.stringValue + ".asset");
            AssetDatabase.SaveAssets();
            selectedSTANK = null;
            RefreshSTANKListView();
        }

        void CreateSTANK(){
            // Toggle the Delete STANK button off
            deleteSTANKButton.ToggleInClassList(hiddenClassName);
            // Toggle the Save STANK button on
            saveSTANKButton.ToggleInClassList(hiddenClassName);

            selectedSTANK = ScriptableObject.CreateInstance("Stank") as Stank; 
            selectedSTANK.Name = "New Stank";
            
            serializedSelectedSTANK = new SerializedObject(selectedSTANK as UnityEngine.Object);
            iconProperty = serializedSelectedSTANK.FindProperty("Icon");
            stankNameProperty = serializedSelectedSTANK.FindProperty("Name");
            descriptionProperty = serializedSelectedSTANK.FindProperty("Description");            
            gizmoColorProperty = serializedSelectedSTANK.FindProperty("GizmoColor");

            iconField.BindProperty(iconProperty);
            stankNameField.BindProperty(stankNameProperty);
            stankDescriptionField.BindProperty(descriptionProperty);
            gizmoColorField.BindProperty(gizmoColorProperty);

            RefreshSTANKListView();
        }

        void CreateSTANKResponse(){
            
            deleteSTANKResponseButton.ToggleInClassList(hiddenClassName);
            saveSTANKResponseButton.ToggleInClassList(hiddenClassName);
            selectedSTANKResponse = ScriptableObject.CreateInstance("STANKResponse") as STANKResponse;
            
            serializedSelectedSTANKResponse = new SerializedObject(selectedSTANKResponse as UnityEngine.Object);
            stankResponseNameProperty = serializedSelectedSTANKResponse.FindProperty("Name");
            pungencyThresholdProperty = serializedSelectedSTANKResponse.FindProperty("PungencyThreshold");
            responseDelayProperty = serializedSelectedSTANKResponse.FindProperty("ResponseDelay");
            AnimationClipProperty = serializedSelectedSTANKResponse.FindProperty("AnimationClip");
            stankResponseNameField.BindProperty(stankResponseNameProperty);
            pungencyThresholdField.BindProperty(pungencyThresholdProperty);
            responseDelayField.BindProperty(responseDelayProperty);
            AnimationClipField.BindProperty(AnimationClipProperty);

            RefreshSTANKResponseListView();
        }

        void SaveSTANKResponse(){
            deleteSTANKResponseButton.ToggleInClassList(hiddenClassName);
            saveSTANKResponseButton.ToggleInClassList(hiddenClassName);
            Debug.Log("Saving response");
            AssetDatabase.CreateAsset(selectedSTANKResponse, responseSavePath + selectedSTANKResponse.Name + ".asset");
            AssetDatabase.SaveAssets();
            RefreshSTANKResponseListView();
        }

        void BuildSTANKResponsesTab() {
            stankResponseNameField = stankBankTab_STANKResponses.Q<TextField>("ResponseNameField");
            stankResponseDescriptionField = stankBankTab_STANKResponses.Q<TextField>("ResponseDescriptionField");
            stankResponseListView = stankBankTab_STANKResponses.Q<ListView>("STANKResponseListView");            
            stankResponseListView.selectionChanged += OnSTANKResponseSelectionChange;
            responseStankField = stankBankTab_STANKResponses.Q<ObjectField>("STANKField");
            pungencyThresholdField = stankBankTab_STANKResponses.Q<FloatField>("PungencyThresholdField");
            responseDelayField = stankBankTab_STANKResponses.Q<FloatField>("ResponseDelayField");
            AnimationClipField = stankBankTab_STANKResponses.Q<ObjectField>("AnimationClipField");
            if(AnimationClipField == null) Debug.Log("AnimationClipField not found");
            deleteSTANKResponseButton = stankBankTab_STANKResponses.Q<Button>("DeleteSTANKResponseButton");
            createSTANKResponseButton = stankBankTab_STANKResponses.Q<Button>("CreateSTANKResponseButton");
            saveSTANKResponseButton = stankBankTab_STANKResponses.Q<Button>("SaveSTANKResponseButton");
            createSTANKResponseButton.clicked += CreateSTANKResponse;
            saveSTANKResponseButton.clicked += SaveSTANKResponse;
            deleteSTANKResponseButton.clicked += DeleteSTANKResponse;
        }

        void BuildSmellersTab() {
            smellersListView = stankBankTab_Smellers.Q<ListView>("SmellersListView");
            smellerStankField = stankBankTab_Smellers.Q<ObjectField>("SmellerStankField");
            smellerRadiusField = stankBankTab_Smellers.Q<FloatField>("SmellerRadiusField");
            pungencyCurveField = stankBankTab_Smellers.Q<CurveField>("PungencyCurveField");
            expansionRateField = stankBankTab_Smellers.Q<FloatField>("ExpansionRateField");
            showStankLinesToggle = stankBankTab_Smellers.Q<Toggle>("ShowStankLinesToggle");
            stankLinesEmittersField = stankBankTab_Smellers.Q<PropertyField>("StankLinesEmittersField");
        }

        void BuildFellersTab() {
            
        }

        void ShowSTANKSTab(){
            RefreshSTANKListView();
            stankBankTab_STANKS.ToggleInClassList(hiddenClassName);            
            stankBankTab_STANKResponses.ToggleInClassList(hiddenClassName);            
        }

        void ShowSTANKResponsesTab(){
            RefreshSTANKResponseListView();
            stankBankTab_STANKResponses.ToggleInClassList(hiddenClassName);            
            stankBankTab_STANKS.ToggleInClassList(hiddenClassName);            
        }

        public void CreateGUI()
        {
            if(rootAsset == null) Debug.Log("rootAsset not found");
            stankListView = rootAsset.Q<ListView>("STANKSListView");
            RefreshSTANKListView();
            //ShowSTANKSTab();
            UpdateHUDImagePreview();
        }       

        private void OnSTANKSelectionChange(IEnumerable<object> selectedItems)
        {            
            // Update the window when we change STANK selections
            selectedSTANK = selectedItems.First() as Stank;
            serializedSelectedSTANK = new SerializedObject(selectedSTANK as UnityEngine.Object);
            iconProperty = serializedSelectedSTANK.FindProperty("Icon");
            stankNameProperty = serializedSelectedSTANK.FindProperty("Name");
            descriptionProperty = serializedSelectedSTANK.FindProperty("Description");            
            gizmoColorProperty = serializedSelectedSTANK.FindProperty("GizmoColor");

            if(iconField != null && iconProperty != null) iconField.BindProperty(iconProperty);
            else if(iconProperty == null)Debug.Log(("spriteProperty not found"));
            else if(iconField == null)Debug.Log("iconField is null");            
            if(stankNameProperty != null) stankNameField.BindProperty(stankNameProperty);
            else Debug.Log(("nameProperty not found"));
            if(descriptionProperty != null) stankDescriptionField.BindProperty(descriptionProperty);
            else Debug.Log(("descriptionProperty not found"));
            if(gizmoColorProperty != null) gizmoColorField.BindProperty(gizmoColorProperty);
            else Debug.Log(("gizmoColorProperty not found"));

            UpdateHUDImagePreview();
        } 

        private void OnSTANKResponseSelectionChange(IEnumerable<object> selectedItems)
        {            
            // Update the window when we change STANK selections
            selectedSTANKResponse = selectedItems.First() as STANKResponse;
            serializedSelectedSTANKResponse = new SerializedObject(selectedSTANKResponse as UnityEngine.Object);

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

        [MenuItem("Tools/STANK/STANKBank")]
        public static void ShowWindow()
        {
            STANKBank wnd = GetWindow<STANKBank>();
            wnd.titleContent = new GUIContent("STANKBank");
            wnd.Focus();
            Rect windowRect = new Rect(500f, 500f, 300f, 450f);
            wnd.position = windowRect;
            wnd.minSize = new Vector2(750f, 250f);            
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
            var allSTANKGuids = AssetDatabase.FindAssets("t:Stank");
            allSTANKs.Clear();
            foreach (var guid in allSTANKGuids)
            {
                allSTANKs.Add(AssetDatabase.LoadAssetAtPath<Stank>(AssetDatabase.GUIDToAssetPath(guid)));
            }

            // This adds the new STANK to the ListView when the user creates a new STANK
            if(!allSTANKs.Contains(selectedSTANK) && selectedSTANK != null) {
                allSTANKs.Insert(0, selectedSTANK);
                isNewSTANK = true;
            } else {
                // If we create a new STANK but don't save it, we need to remove it from the list
                allSTANKs.Remove(selectedSTANK);
            }
            CreateSTANKListView();
            stankListView.Rebuild();
            stankListView.AddToSelection(0);
        }

        public void RefreshSTANKResponseListView()
        {
            // if (stankResponseListView == null) Debug.Log("stankResponseListView not found");

            var allSTANKResponseGuids = AssetDatabase.FindAssets("t:STANKResponse");
            allSTANKResponses.Clear();
            foreach (var guid in allSTANKResponseGuids)
            {
                allSTANKResponses.Add(AssetDatabase.LoadAssetAtPath<STANKResponse>(AssetDatabase.GUIDToAssetPath(guid)));
            }
            
            if(!allSTANKResponses.Contains(selectedSTANKResponse) && selectedSTANKResponse != null) {
                allSTANKResponses.Insert(0, selectedSTANKResponse);
                isNewSTANKResponse = true;
            } else {
                allSTANKResponses.Remove(selectedSTANKResponse);
            }
            //Debug.Log("selectedSTANKResponse: "+selectedSTANKResponse.Name);
            // Clear the list view and rebuild it
            CreateSTANKResponseListView();
            stankResponseListView.Rebuild();
            stankResponseListView.AddToSelection(0);
        }

        private void CreateSTANKResponseListView()
        {
            // Initialize the list view with all STANKResponse names
            stankResponseListView.Clear();        
            stankResponseListView.makeItem = () => new Label();
            stankResponseListView.bindItem = (item, index) => { (item as Label).text = allSTANKResponses[index].name; };
            stankResponseListView.itemsSource = allSTANKResponses;
        }

        private void CreateSmellerListView(){
            // Initialize the list view with all Smeller names
            smellersListView.Clear();        
            smellersListView.makeItem = () => new Label();
            smellersListView.bindItem = (item, index) => { (item as Label).text = allSmellers[index].name; };
            smellersListView.itemsSource = allSmellers;
        }

        public void RefreshSmellerListView()
        {
            // if (stankResponseListView == null) Debug.Log("stankResponseListView not found");

            var allSmellerGuids = AssetDatabase.FindAssets("t:Smeller");
            allSmellers.Clear();
            foreach (var guid in allSmellerGuids)
            {
                allSmellers.Add(AssetDatabase.LoadAssetAtPath<Smeller>(AssetDatabase.GUIDToAssetPath(guid)));
            }

            // Clear the list view and rebuild it
            CreateSmellerListView();
            smellersListView.Rebuild();
            smellersListView.AddToSelection(0);
        }

        private void CreateFellerListView(){
            // Initialize the list view with all Smeller names
            fellersListView.Clear();        
            fellersListView.makeItem = () => new Label();
            fellersListView.bindItem = (item, index) => { (item as Label).text = allFellers[index].name; };
            fellersListView.itemsSource = allFellers;
        }

        public void RefreshFellerListView()
        {
            // if (stankResponseListView == null) Debug.Log("stankResponseListView not found");

            var allFellerGuids = AssetDatabase.FindAssets("t:Feller");
            allSTANKResponses.Clear();
            foreach (var guid in allFellerGuids)
            {
                allFellers.Add(AssetDatabase.LoadAssetAtPath<Feller>(AssetDatabase.GUIDToAssetPath(guid)));
            }

            // Clear the list view and rebuild it
            CreateSTANKResponseListView();
            fellersListView.Rebuild();
            fellersListView.AddToSelection(0);
        }

        private void UpdateHUDImagePreview()
        {
            // If the sprite is not null, set the background image to the sprite
            // If the sprite is null, populate it with our default grid image
            if(iconField == null) { Debug.Log("iconField not found"); return;}
            if(stankHudSpriteField == null) { Debug.Log("stankHudSpriteField not found"); return;}
            if (iconField.value != null)
            {
                stankHudSpriteField.style.backgroundImage = iconProperty.objectReferenceValue as Texture2D;
            }
            else
            {
                if(defaultImageGridTexture == null) {Debug.Log("defaultImageGridTexture not found"); return;}
                stankHudSpriteField.style.backgroundImage = defaultImageGridTexture;
            }
            
        }
    }
    
}