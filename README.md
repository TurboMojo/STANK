
![GithubBanner](https://github.com/TurboMojo/STANK/assets/168988176/f4d93f31-98cf-4771-a0c3-fd3de649261b)

**STANK**

Smell Tracking and Notification Kit is a Unity plugin that gives you the tools to add a new dimension of immersion to your game by giving your players and NPCs a sense of smell! 

The Stank is the basic working unit of STANK.  It defines a smell and its properties.  It is expressed into the world by a Smeller.  Fellers can smell smellers and react to them in ways defined by a STANKReaction.

The Feller can detect and react to those scents in a variety of ways. You can have your character blissfully stop and smell the roses at a flower stand or wretch and vomit at the stench of a dumpster. STANK facilitates these types of interactions and gives you the tools to be creative with your applications of the concept.

STANK can trigger Canvas icons, sound effects, animations, and particle systems out of the box.  These interactions are defined in a STANKResponse.  A Feller may have multiple reponses for each Stank and respond according to the Pungency, or intensity, of the Stank as perceived by the Feller.  

**BASIC WORKFLOW**

1. Add the Olfactory prefab to your scene.

2. Add a Smeller prefab as a child to any GameObject that should be smelled.

3. Create and configure a Stank from the right-click menu in the Project panel.
   
   - Pungency is the ratio of the currently applicable Stanks on a Feller to said Feller's tolerances.  1.0 represents the largest threshold defined for the Stank on the Feller.  Not visible in the inspector, since it is always set in code.  It is, however, important to know how Pungency works when designing your STANKscape.
   
   - Gizmo color changes the color of the debug sphere in the editor for any Smellers using this Stank.
   
   - Icon and HUDMaterial are used for display of a HUD Icon if you're using one.

4. Configure the Smeller with the Stank you just created.
   
   - Radius defines how far away Fellers should be able to detect the Stank of the Smeller.
   
   - Stank defines the smell of the smeller.
   
   - Pungency curve defines the Pungency field of the Stank over the Smeller's radius.  The leftmost side of the curve defines the intensity of the smell at the center of the radius.
   
   - Expansion rate determines how quickly the smell expands once the Smeller comes into existence.
   
   - The ShowStankLines boolean defines whether you plan on using visible stink lines in the form of particle systems.  
   
   - Stank Lines Emitters is an array of particle systems that should be displayed as stink lines.

5. Add the Feller prefab as a child to any GameObject that should be aware of smells.  A Feller is the driving force for anything to detect Stanks in the scene.  It calculates the current pungency of any Smellers in range and compares it to the thresholds set in a Feller's STANKResponses for that Stank.  

6. Create a STANKResponse for this Feller for this Stank and configure it with any animations or sound clips you wish to play during this response.  STANKYLeg builds a Mecanim layer automatically and switches to it whenever a STANKResponse is triggered.  You can provide an AvatarMask, if you wish to only animate portions of your mesh for this STANKResponse.

7. Add the STANKResponse to the Inspector field for your Feller.

8. Depending on the role of this Feller, you'll add either AutoSniffer or FellerPlayerInput to the Feller gameobject.
   
   - AutoSniffer forces the feller to smell at regular intervals.  If the Feller should always be updating its awareness, set the SniffInterval to 0.  This can be used to simulate breathing cadence.
   
   - FellerPlayerInput allows the act of sniffing to be triggered by player input.

At this point, you have the basic configuration for a Feller to be aware of and respond to the Smellers in your scene.  

Both STANKEye and STANKYLeg are optional components that you can add as necessary. Their actions are triggered either internally or via the ProcessThreshold() method, which is called by the Feller on any components derived from STANKResponseListener.

STANKEye manages the visual portions of any STANKResponse.  This includes particle systems and UI for now, but support for VFX graph, shader graph, and other features are planned.

STANKYLeg manages physical reactions to a STANKResponse, like animations and ragdolls.  STANKYLeg expects animations to be driven by a standard AnimatorController and builds a custom layer from Animations defined in your Reactions and triggers those animations when appropriate.  No additional setup of your AnimatorController is necessary.  You do have the option of providing an AvatarMask, if you wish to mask parts of the body that will be animated by STANKYLeg.

**STANKEye**

STANKEye is used for displaying visual information about STANKs and a Feller's perception of them.  

1. Add STANKEye to your Feller gameobject.  

2. Add the STANKResponseParticlesPrefab as a child of the STANKEye gameobject and configure it with the relevant Stank.

3. Configure the ParticleSystem on the STANKResponseParticlesPrefab to provide the VFX you want to play for a STANKReaction.

**STANKYLeg**

Add the STANKYLeg component to your Feller.

Create and configure at least one STANKResponse.

1. The AnimationClip will be included and triggered by STANKYLeg to build an animation layer used by STANK when this reaction is triggered.

2. ResponseVFX is a particle system that simulates something happening in the world, such as vomit or a sneeze.

3. ResponseScreenVFX is where you should use particle systems meant for the first-person player.  This is for things like tears when cutting onions, toxic haze when near a sewer, and other things that the player themselves would experience.

4. Response Audio is an array of AudioSources providing sound effects that should be played when a Response is triggered.



**CUSTOM EXTENSIONS**

Custom classes should inherit STANKResponseListener and implement the ISTANKResponse interface.  This will include your script in the Event System driving STANKResponses.  The ISTANKResponse Interface will give you the ProcessThreshold(STANKResponse response) method, which will be called any time a STANKResponse is triggered.
