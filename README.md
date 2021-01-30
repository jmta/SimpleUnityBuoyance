# SimpleUnityBuoyance

A rather simplistic Buoyancy model in Unity. 
Knowledge gathered from https://www.youtube.com/watch?v=eL_zHQEju8s and https://www.youtube.com/watch?v=OOeA0pJ8Y2s&t=1351s 

I hope this find a nice middle ground between providing a simple solution but also provides a relatively accurate simulation.

### Usage 

The 'sea' object must have a WaterManager script attached to it.
The buoyant object reqiures a Box Collider and a RigidBody. Adjusting the floatation points will increase the accuracy but at the cost of additional calculations. Playing around with Total Floatation force, and the RigidBodies Mass, Drag and Angular Drag are the key to a better simulation. 


### Useful Methods

#### pointUnderwater
The water manager (singleton) has a `'pointUnderwater'` method which can inform you if a specific point (in global co-ordinates) is underwater, this can be used for things such as only providing thrust if the point is under the water

