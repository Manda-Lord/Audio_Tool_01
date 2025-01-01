#### NOTE
The commits shown in my repo are not indicative of the actual commits I have pushed. Unfortunately I ran into some issues with my previous repo of the same name once the file size exceeded 50MB. The problem lied in the fact that I was operating under an OS system, which prevented my commits from pushing into Github Original / Main. To fix the issue I had to manually initialise the LFS under the Repository tab in Fork, clone this repo into its own standalone project within Fork. At this point I could see the two same Repos, but one tab had all my commits that failed to push, and the other only had the 8. I opened up the folder location for both and transferred over all of the file --including the hidden files but excluding the .git file-- from the broken repo to the "new" repo. The commit history then became visible in the new Cloned repo, which I then staged all 989 files, made sure the LFS was initialised and then committed + pushed into Original / Main. Here is a link to a folder with screenshots of my commit history as evidence to support my work: https://drive.google.com/drive/folders/1JPaf_MOaleFA8A7FkYwMT99eauqOg86v?usp=sharing

#### LINKS
3-Minute Diorama Presentation: https://drive.google.com/file/d/1kgEmgYJGplka2nhzkGccccPgxaot_LKg/view?usp=sharing

#### ASSETS
Metal, Impact, Heavy, Powerful, Big, Slide, Movement, Variations 01 SND117149 from Soundly: https://drive.google.com/file/d/1N49KrXMKoTbUBVfS21jkiPQ4NbFjSRgq/view?usp=sharing

#### Screenshots
This folder contains screenshots of what is happening behind the scenes of my audio tool with raycasts. Folder contains images of different stages of its development:
https://drive.google.com/drive/folders/1TqgBJS-BmND_W4hCyO_1kU0Nm_pv14k3?usp=sharing

# The Proposal
Core skills
- Sound design.
- Level design.
- Audio programming.

My proposed project is an audio solution designed to achieve a more realistic and immersive auralisation of diegetic sounds. This system in context with the in-engine diorama will double as an interactive acoustics simulator, allowing the user to engage with environment geometry to observe aurally how sound is propagating through the space and reflecting off of surfaces. "Noisemakers" - audio emitters with various sound loops designed by myself - will be placed in specific locations within the space, with some Noisemaker Drones also flying via a pre-defined path.

**Key requirements** : The MVP of my project will be the level design and layout, 3D asset imports, simple environment interaction, Noisemaker placement and sound wave propagation / environment geometry interaction. If time permits, complexity will be added; audio data visualisation using gizmos and / or ray-casting / raytracing, VFX to improve the visuals, and a UI for the user to engage with Noisemakers.

Third-party assets will include some sounds from Soundly if my library lacks them or I’m unable to record due to time or feasibility constraints. The level will use basic shapes, prioritising form and function. Prefabs and models will be imported from the asset store.

### License Defense ###
This is my first serious attempt at creating a functioning tool which I will be able to utilise during my time developing in the Games Academy with my peers. As a Sound Designer, I hope this tool will augment the realism of audio within whatever games I end up working in during my time at University. Once the tool is fully working as desired, I wish to pass it on to my Sound Designer cohort from my previous year studying the Sound Design course. They will be in Study Block 2 by the time I'm finished with it, so hopefully they can make use of it if they so wish to.

For that reason it makes sense to use the MIT License, it's relatively simple and will allow my tool to be freely used and adapted by others in the Games Academy if soundscape immersion is a feature they wish to implement. This isn't a novel idea, although when I first thought of it I was ignorant of the fact, there are already - unsurprisingly - sound spatialising tools out there; such as Steam Audio, that offers the very same solution completely free of charge. So there is no real risk of commercialisation if this tool is successful in its design and execution.

## Reflection & Documentation

### SoundOcclusion.cs
Really glad to have walked away from the project for a good couple weeks, upon my return I was able to identify the bug in my code. Before I dive into that, I want to first introduce this script. The aim of this script is to simply enable the total occlusion of sound upon reaching a wall count threshold, which in my case is 3.

I’ve included 7 public fields:
Listener - Position of audio listener,
lowPassFilter - (removes high frequencies),
audioSource - Plays the sound (from a Noisemaker),
maxCutoffFrequency / minCutoffFrequency - Defines the LP frequency range,
maxVolume / minVolume - Controls the volume range,
transitionSpeed - How fast the filter and volume adjust,
occlusionLayer - Specifies which layers represent occluding objects

And 3 private fields:
_targetCutoffFrequency / _targetVolume - Target values for smooth transitions,
wallCount - Tracks the number of walls occluding the sound,
previousWallCount - Tracks the wall count in the last frame for debugging. There was a reason I had to do this at the time of debugging, but I forgot why exactly.

First maxCutoffFrequency and maxVolume are initialised to ensure the sound starts unobstructed. Then the compiler runs Update() which resets the wallCount (of occluding walls) every frame. A raycast is also fired from the sound source to the listener and collects all hits (objects on the occlusionLayer) in RaycastHit[] hits on objects tagged “Occluder”. Initially the counter would stop at 3 walls, and this was the first bug identified that was causing potential issues with the behaviour of this script.

—
The Bug
Made a few key changes in the logic, first the wallCount >=3 check. First I tried to force the counter to stop incrementing, but for some reason,my Debug.Log was displaying a count of walls exceeding 3. Initially I was going to clamp wallCount to 3 using the Mathf.Clamp(wallCount, 0, 3); However the statement became redundant due to the condition if (wallCount >3) break. This line ensures that wallCount can never exceed 3. However for some reason wallCount was still incrementing, which means something outside of the loop is causing the wallCount, signalling a logical flaw. Rather then working out what it was, simply allowing the counter to exceed 3 and designing the logic around that would make life easier. The aim of the occluder is to completely attenuate (eventually to complete 0 value for the “Volume off” effect) once we hit 3 wallCount. Anything beyond that would also be volume off. So instead, I just needed to handle any situation that exceeds 3 and not worry about clamping.

The Mathf.Lerp is still applied for smooth transitions, but the safeguard ensures the volume reaches 0 precisely when needed. In case it doesn’t fully reduce volume to 0 due to floating-point imprecision, a hard check is added if (wallCount >= 3 &&  Mathf.Approximately(audioSource.volume, 0f)) { audioSource.volume = 0f }.

UPDATE

Okay that didn’t work. Maybe the smooth transition via Mathf.Lerp may be delaying the volume from reaching 0. So instead I will bypass that and replace it with a direct assignment of 0f.

MORE BUGS

Okay now the cut-off is too abrupt, because Im directly setting the volume to 0 when wallCount >= 3. I need to use Mathf.Lerp to transition the volume and cutoff frequency smoothly.

The other bug is that the audio stays permanently muted. This is occurring because the audioSource.volume is hardcoded to 0, with no logic to reset it to the target volume when the wallCount decreases below 3. I will apply fixes now.

—

Lastly, I added proximityFactor which calculates a normalised value (0-1) to determine how far the listener is from the center of the wall in order to smoothly transition cutoff frequency and volume gain from proximity-based adjustments. At the center of the wall, maximum occlusion would occur, and at the edges of the wall, lesser occlusion would occur. Simple logic to improve its realism factor.


### SoundReflection.cs
Fully simulating how sound reflects, absorbs and diffuses in the real-world would be computationally intensive. However, a simplified system with limited reflections and material-based reverb adjustments is a good compromise that will still create an immersive audio experience.

SoundReflection should continually adjust the ray path by reflecting off surfaces, with the goal of dynamically reconnecting to the listener whenever possible. Each reflection should actively seek a path to the listener. 

### Plan of Execution
- Raycast with reflections
 First I will be using the raycast to reflect off of surfaces, in order to simulate the path of sound reflection. The reflection path can be calculated based on the angle of incidence using vector mathematics.
 Use the ‘Vector3.Reflect’ function to get the reflected direction e.g. ‘Vector3 reflectedDirection = Vector3.Reflect(ray.direction, hit.normal);
 A new ray can then be cast from the hit point in this reflected direction to simulate the first reflection, and repeat for additional reflections (3-4 times).
 Adding reverb for each reflection
 As each reflection occurs, increase the reverb or echo effect subtly. This can be done by adding a small reverb increment based on the reflection count. (Material type will have an effect by what amount, but first I need to get this mechanism working, then I will include another material type in the computation)
 Raycast must dynamically reconfigure to reach the listener if an obstacle blocks the direct path. The ray should find another route by calculating alternative reflections off other surfaces that could lead to the listener, i.e. only calculate instances where paths reach the listener through direct or reflected rays. This will filter out irrelevant objects and streamline computation.
 If reflecting off the obstacles would result in a path that could never reach the listener, that reflection should be disregarded.
 Will use a simple algorithm to adjust the reverb intensity and volume attenuation with each bounce, i.e. reverb increases slightly with each reflection, while volume decreases proportionally to simulate the natural energy loss of sound in real spaces.

- Material properties for surfaces
 Assign material properties to each surface, defining how much sound it reflects or “absorbs”.
 Default (imported assets) have a visually metallic like surface. So this will have a higher reflective (reverb) factor than other material types.
 Will create an extra material type like cloth or wood (perhaps both, depends on time constraints). These will absorb (attenuate + low pass), reducing reflection and causing faster attenuation.
 Furniture covered with cloth might be included in my scene, once I import the asset from Unity store. These could have an interesting effect on a Noisemaker within a confined room full of covered furniture. A duplicate room will be created with the same Noisemaker but without furniture for comparison.
 This can be achieved through simple properties on each surface’s script, by defining an absorption factor or reverb multiplier that adjusts how the sound behaves upon reflection.

- Performance considerations
 Limiting to 3-4 reflections prevents excessive computation and reverb stacking. This will have to be adjusted by testing; 4 might not be enough for distances where raycast could reach the player. This would be a powerful tension building device in games like Alien: Isolation, where the movement of the Xenomorph could be accurately detected by the player a few corridors down.
 For large complex environments, could consider using a hybrid approach; combining occlusion for distant obstacles and limited reflections for nearby surfaces to simulate close reflections.

- Mathematical considerations
 This feature will rely on the Law of Reflections, where the angle of incidence equals the angle of reflection. For a ray hitting a surface at a 45-degree angle, it will reflect off at the same angle on the opposite side of the normal.

### Algorithm
- Initial Raycast
 Start with a raycast from the sound source toward the listener.
 If it hits the listener directly, no further reflections are needed.
 If the raycast hits an obstacle, move to reflection processing.
 Reflected Raycast with Listener check
 If an obstacle is hit, use Vector3.Reflect to calculate the reflection angle.
 Cast the reflected ray from the hit point in the new direction.
 For each subsequent reflection (3-4), check if the ray intersects with the listener.
 Only continue the reflections if the path has a chance of reaching the listener.
 Each reflection should be calculated until it either reaches the listener or exceeds the maximum reflection count.
 Only draw the reflection path if it eventually leads to the listener; otherwise, stop processing that path.
 If a reflected path hits the listener, process the sound effect (reverb, attenuation) and stop further reflections.
 Filtering unnecessary objects
 Set up the LayerMask for the initial and reflected raycasts to only include relevant layers (“Reflector”).
 This ensures the raycast ignores non-essential objects and keeps focus on surfaces that could realistically affect sound reflection.
 Implement visibility checks
 For any detected object along the path, confirm that it’s within the ray’s line of sight.
 Use RaycastHit to determine if an obstacle in the path fully or partially blocks the listener.
 If blocked, reduce or ignore the effect for that specific reflection.

### Fields
 In Unity, the reverb properties in audio (such as reverbZoneMix) don’t directly correspond to real-world units of measurement like decibels or seconds. Instead, Unity uses normalised values (from 0 to 1) to represent level or intensities in various audio parameters. Below is how each of the parameters work for SoundReflection in relation to Unity’s audio system :

- Base Reverb (0 to 1)
 The ‘baseReverb’ parameter is a normalised value that ranges from 0 (no reverb) to 1 (full reverb).
 A value of 0 means the dry signal only.
 A value of 1 means the maximum wet signal from the reverb effect.
 Intermediate values (e.g. 0.5) represents a mix of the dry and wet signals, providing a moderate reverb effect.
 This parameter essentially controls how much of the reverb effect is applied in the overall sound mix.
 Min and Max Reverb (0 to 1)
 This defines the range for the reverb effect’s intensity.
 Setting minReverb = 0 means you allow the sound to be completely dry when needed, while maxReverb = 1 allows the maximum possible reverb for a fully wet signal.
 Adjustments can be made to these values to limit how dry or wet the sound can get. For example, setting minReverb = 0.3 and maxReverb = 0.8 would prevent the reverb from going fully dry or fully wet.
 Reflection Reverb Increment
 This controls how much additional reverb is added with each reflection, and it increments by the specified amount with each reflection count.
 If RRI is 0.1, then with each additional reflection, the reverb increases by 0.1 in the normalised scale (0 to 1).
 I.e. first reflection: base reverb + 0.1; second reflection: base reverb + 0.2; this continues until it reaches ‘maxReverb’ limit.
 Update
 Major changes had to be made to this script in order for it to work correctly. Minor changes were integrated into the README by updating sections above, however due to the changes gradually becoming more complex, a more in-depth description is in order.

Initially, only a single ray was cast from the Noisemaker, but each time I couldn’t get that specific ray to reflect accurately towards the listener. It would bounce off of surfaces but it wasn’t capable of seeking out the player. My idea was pretty simple - to maintain the Listener to Noisemaker ray pathway connected, if there were any obstacles, to immediately recalculate a new path using reflections off of other surfaces to successfully “re-attach” to the Listener - but Unity was unbelievably stubborn in not doing what I asked of it. So the next thing I decided after a few iterations and unexpected behaviours, was to alter my logic. 

Instead of seeking out the Listener, have one raycast that always remains connected to the Listener (basically the default raycast) which will apply a Base Reverb of around 0.5 (an even mix of dry and wet signals) only when that raycast detects an obstacle (meaning some kind of reverb would apply naturally. Then have a cone spread of rays which will seek out obstacles first, and then reflect. Multiplying the amount of rays cast increases the probability of rays hitting the Listener. And this worked! With that, I added a counter ‘successfulRayCount’ to keep track of the number of reflections before hitting the player, and for each reflection, increment the amount of extra reverb in addition to the Base Reverb.
Conditional break in CastSphericalSpreadRays.
If successfulRayCount reaches maxSuccessfulRays, stop casting additional rays.
Conditional rever application in CastReflectiveRay
Which is only applied if successfulRayCount is within the allowed maximum.
So when a player goes deeper into a tunnel, say, then the amount of reverb would noticeably increase. This would of course depend on the material type but I’m not going to attempt that just yet, to keep the diorama relatively simple for now. I just want occlusion and reverberation to work as intended for my MVP.

What brought about this thinking was when I had the Listener central within two walls that were running parallel to each other. When I placed the Noisemaker in the direct line of sight of the player, no reverberation would be added because it didn’t detect any obstacles. Which made me think, in real life, sound omnidirectionally hits all surfaces - and we of course only hear whatever direct and reflected soundwaves make it to our ears. This is what brought about my next solution to have the direct rays seek out obstacles regardless of whether an obstacle is detected or not.
With the added capabilities of placing a limiter Max Successful Rays.
‘maxSuccessfulRays’ defines the maximum number of rays that’s needed before it’s supposed to stop further raycasts. However for some reason it continues to cast out rays, this might have something to do with its relationship per update per frame. Fixing this might not be necessary if raycasts aren’t computationally intensive.
So long as when the max is reached, sound effects are only applied to that amount of Rays * the reflections for each ray.
This will avoid overloading the computation unnecessarily with overprocessing.
For Future Reference
I’ve identified a key limitation within my setup for this project. Well, namely with Unity’s Audio Reverb Zone system. I assumed I would be able to adjust the shape of the ARZ to the dimensions of a room, however the sizing is limited to spherical dimensions, which makes it difficult to implement into a space effectively. Also, the logic of the Minimum and Maximum Distance fields in the Inspector for ARZ doesn’t make any sense. Reverb in real life has an absolute termination value, what I mean by that is, the reverb is a reaction to the physical obstacles within a space. An example: if you were to be in a cave that’s 100m^3-- and say in real life God implemented ARZ in the same way Unity does --If my Minimum to Maximum differential was 30m^3, then the moment I start to walk beyond the 50m mark, the reverb would fade in terms of wetness to dryness ratio. Whereas in real life what we would notice instead is a decay in the reverb over x distance.
The way I will tackle this in the future is either I make sure my scripts are designed without being dependent on the ARZ.
Or I create my own ARZ, with various basic shapes, like the cone, cube, prism, etc.
And make sure that increase in distance affects decay, not the reduction of wetness.
Magnitude would affect the tail of sounds, which should also be taken into account.

The other limitation is the relationship of the space -> to the listener -> to the audiosource is also flawed. The reverb effect is applied to the Audio Source itself (the Noisemaker), based on its position within the reverb zone, rather than being influenced by the listener's position. So if the Noisemaker is static AND outside the ARZ, it won’t experience reverb, even if the listener moves through zones that should theoretically have an effect on how the sound is heard.
