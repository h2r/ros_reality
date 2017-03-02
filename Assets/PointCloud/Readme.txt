Point Cloud Viewer ------------------------------------

This is a follow-up from this web page (Retrieved 15th July 2014):
http://www.kamend.com/2014/05/rendering-a-point-cloud-inside-unity/

This asset allows to import and visualize PointCloud data (OFF). It creates several meshes of 65000 vertices which groups them in a  final prefab.

After ~10-15 million points the frame rate starts to drop.

_______________________________________________

Instructions:
	Add an empty Game Object in the Scene.
	Add the script "PointCloudManager.cs" to this object.
	In this script component:
		Write the path of your PointCloud file without the extension (e.g. "/PointCloud/myPointCloudFile")
		Assign the material 'VertexColor' to 'Mat Vertex'
		Choose other parameters such as scale, invert YZ and force reload.
	Run the scene -> The script will automatically store the meshes and the prefab in the Resources folder. If the script is executed again it will try to upload the stored prefab, unless the option 'Force Reload' is checked.
	
Additional:
According to the webpage, an additional script ("EnablePointSize") should be attached to the main camera.

This demo contains a Point Cloud ("xyzrgb_manuscript") from the Stanford Computer Graphics Laboratory which is not to be used for commercial purposes, nor should it appear in a product for sale (with the exception of scholarly journals or books), without their permission.