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
        private const string currentlySelectedTabClassName = "selected";
        private const string unselectedContentClassName = "hidden";

        // Elements of the UIDocument
        VisualElement rootAsset;
        Texture2D defaultImageGridTexture;
        
        UIDocument stankBankAsset;    
        List<Stank> allSTANKs = new List<Stank>();
        List<STANKResponse> allSTANKResponses = new List<STANKResponse>();    
        List<Smeller> allSmellers = new List<Smeller>();
        List<Feller> allFellers = new List<Feller>();
        

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
        SerializedProperty spriteProperty;
        Button deleteSTANKButton;
        Stank selectedSTANK;
        SerializedObject serializedSelectedSTANK;

        // STANKResponse tab elements
        ListView stankResponseListView;
        STANKResponse selectedSTANKResponse;
        SerializedObject serializedSelectedSTANKResponse;
        SerializedProperty nameProperty;
        SerializedProperty descriptionProperty;    
        SerializedProperty gizmoColorProperty;
        TextField stankNameField;
        TextField stankDescriptionField;    
        ColorField gizmoColorField;
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
            //BuildFellersTab();
            //BuildSmellersTab();

            spriteProperty = null;
        }

        void BuildToolBar(){
            
            stanksButton = rootAsset.Q<ToolbarButton>("STANKsToolbarButton");            
            stankResponsesButton = rootAsset.Q<ToolbarButton>("STANKResponsesToolbarButton");            
            smellersButton = rootAsset.Q<ToolbarButton>("SmellersToolbarButton");            
            fellersButton = rootAsset.Q<ToolbarButton>("FellersToolbarButton");            

            stanksButton.clicked += ShowSTANKSTab;
            stankResponsesButton.clicked += ShowSTANKResponsesTab;
            smellersButton.clicked += ShowSmellersTab;

        }

        void BuildBANKWindow(){
            VisualTreeAsset stankBankDocument = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/STANK/Editor/STANKBank/ImprovedSTANKBank.uxml");
            // Load the elements of the UIDocument
            rootAsset = stankBankDocument.CloneTree();
            if(rootAsset == null) Debug.Log("rootAsset not found");
            rootVisualElement.Add(rootAsset);            
            stankBankTab_STANKS = rootAsset.Q<VisualElement>("STANKsDetails");  
            if(stankBankTab_STANKS == null) Debug.Log("stankBankWindow_STANKS not found");          
            stankBankTab_STANKResponses = rootAsset.Q<VisualElement>("STANKResponsesTab");
            stankBankTab_Smellers = rootAsset.Q<VisualElement>("SmellersTab");
            stankBankTab_Fellers = rootAsset.Q<VisualElement>("FellersTab");
        }

        void BuildSTANKSTab(){
            if(stankBankTab_STANKS == null) Debug.Log("stankBankWindow_STANKS not found");
            stankListView = rootAsset.Q<ListView>("STANKSListView");
            stankListView.selectionChanged += OnSTANKSelectionChange;
            stankBankTab_STANKS = rootAsset.Q<VisualElement>("STANKsDetails");            
            if(stankBankTab_STANKS == null) Debug.Log("stankBankWindow_STANKS not found");            
            iconField = stankBankTab_STANKS.Q<ObjectField>("IconField");
            if(iconField == null) Debug.Log("IconField not found");
            stankHudSpriteField = stankBankTab_STANKS.Q<ObjectField>("IconField");
            stankNameField = stankBankTab_STANKS.Q<TextField>("NameField");
            stankDescriptionField = stankBankTab_STANKS.Q<TextField>("DescriptionField");
            gizmoColorField = stankBankTab_STANKS.Q<ColorField>("GizmoColorField");
            hudMaterialField = stankBankTab_STANKS.Q<ObjectField>("HUDMaterialField");            
            stankHudSpriteField = rootAsset.Q<VisualElement>("IconPreview");
            deleteSTANKButton = stankBankTab_STANKS.Q<Button>("DeleteSTANKButton");
            deleteSTANKButton.clicked += DeleteSTANK;
        }

        void BuildSTANKResponsesTab() {
            stankResponseListView = stankBankTab_STANKResponses.Q<ListView>("STANKResponseListView");            
            stankResponseListView.selectionChanged += OnSTANKResponseSelectionChange;
            responseStankField = stankBankTab_STANKResponses.Q<ObjectField>("STANKField");
            pungencyThresholdField = stankBankTab_STANKResponses.Q<FloatField>("PungencyThresholdField");
            responseDelayField = stankBankTab_STANKResponses.Q<FloatField>("ResponseDelayField");
            AnimationClipField = stankBankTab_STANKResponses.Q<ObjectField>("AnimationClipField");
            if(AnimationClipField == null) Debug.Log("AnimationClipField not found");
            deleteResponsesButton = stankBankTab_STANKResponses.Q<Button>("DeleteSTANKResponseButton");
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
            stankBankTab_STANKS.AddToClassList(currentlySelectedTabClassName);
            stankBankTab_STANKS.RemoveFromClassList(unselectedContentClassName);
            stankBankTab_STANKResponses.RemoveFromClassList(currentlySelectedTabClassName);
            stankBankTab_STANKResponses.AddToClassList(unselectedContentClassName);            
            stankBankTab_Smellers.RemoveFromClassList(currentlySelectedTabClassName);
            stankBankTab_Smellers.AddToClassList(unselectedContentClassName);
            //stankBankTab_Fellers.RemoveFromClassList(currentlySelectedTabClassName);
            //stankBankTab_Fellers.AddToClassList(unselectedContentClassName);
        }

        void ShowSTANKResponsesTab(){
            RefreshSTANKResponseListView();
            stankBankTab_STANKResponses.AddToClassList(currentlySelectedTabClassName);
            stankBankTab_STANKResponses.RemoveFromClassList(unselectedContentClassName);
            stankBankTab_STANKS.RemoveFromClassList(currentlySelectedTabClassName);
            stankBankTab_STANKS.AddToClassList(unselectedContentClassName);
            stankBankTab_Smellers.RemoveFromClassList(currentlySelectedTabClassName);
            stankBankTab_Smellers.AddToClassList(unselectedContentClassName);
            //stankBankTab_Fellers.RemoveFromClassList(currentlySelectedTabClassName);
            //stankBankTab_Fellers.AddToClassList(unselectedContentClassName);
        }

        void ShowSmellersTab(){
            RefreshSmellerListView();
            stankBankTab_Smellers.RemoveFromClassList(unselectedContentClassName);
            stankBankTab_Smellers.AddToClassList(currentlySelectedTabClassName);
            stankBankTab_STANKResponses.AddToClassList(unselectedContentClassName);
            stankBankTab_STANKResponses.RemoveFromClassList(currentlySelectedTabClassName);
            stankBankTab_STANKS.RemoveFromClassList(currentlySelectedTabClassName);
            stankBankTab_STANKS.AddToClassList(unselectedContentClassName);            
            //stankBankTab_Fellers.RemoveFromClassList(currentlySelectedTabClassName);
            //stankBankTab_Fellers.AddToClassList(unselectedContentClassName);
        }

        void ShowFellersTab(){
            RefreshFellerListView();
            stankBankTab_Smellers.RemoveFromClassList(unselectedContentClassName);
            stankBankTab_Smellers.AddToClassList(currentlySelectedTabClassName);
            stankBankTab_STANKResponses.AddToClassList(unselectedContentClassName);
            stankBankTab_STANKResponses.RemoveFromClassList(currentlySelectedTabClassName);
            stankBankTab_STANKS.RemoveFromClassList(currentlySelectedTabClassName);
            stankBankTab_STANKS.AddToClassList(unselectedContentClassName);            
            //stankBankTab_Fellers.RemoveFromClassList(currentlySelectedTabClassName);
            //stankBankTab_Fellers.AddToClassList(unselectedContentClassName);
        }

        public void CreateGUI()
        {
            if(rootAsset == null) Debug.Log("rootAsset not found");
            stankListView = rootAsset.Q<ListView>("STANKSListView");
            
            ShowSTANKSTab();
            UpdateHUDImagePreview();
        }       

        private void OnSTANKSelectionChange(IEnumerable<object> selectedItems)
        {            
            // Update the window when we change STANK selections
            selectedSTANK = selectedItems.First() as Stank;
            serializedSelectedSTANK = new SerializedObject(selectedSTANK as UnityEngine.Object);
            spriteProperty = serializedSelectedSTANK.FindProperty("Icon");
            nameProperty = serializedSelectedSTANK.FindProperty("Name");
            descriptionProperty = serializedSelectedSTANK.FindProperty("Description");            
            gizmoColorProperty = serializedSelectedSTANK.FindProperty("GizmoColor");

            if(iconField != null && spriteProperty != null) iconField.BindProperty(spriteProperty);
            else if(spriteProperty == null)Debug.Log(("spriteProperty not found"));
            else if(iconField == null)Debug.Log("iconField is null");
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
            serializedSelectedSTANKResponse = new SerializedObject(selectedSTANKResponse as UnityEngine.Object);
            if (serializedSelectedSTANKResponse == null)
            {                
                Debug.Log("serializedSelectedSTANKResponse is null");
                return;
            } 
            responseStankProperty = serializedSelectedSTANKResponse.FindProperty("Stank");
            if(responseStankProperty == null) Debug.Log("response StankProperty not found");
            pungencyThresholdProperty = serializedSelectedSTANKResponse.FindProperty("PungencyThreshold");
            if(pungencyThresholdProperty == null) Debug.Log("pungencyThresholdProperty not found");
            responseDelayProperty = serializedSelectedSTANKResponse.FindProperty("ResponseDelay");
            if(responseDelayProperty == null) Debug.Log("responseDelayProperty not found");
            AnimationClipProperty = serializedSelectedSTANKResponse.FindProperty("AnimationClip");
            if(AnimationClipProperty == null) Debug.Log("AnimationClipProperty not found");

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