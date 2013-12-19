using UnityEngine;
using System.Collections;
using System.IO;
 
public class ScreenShot
{
	public string imagePath;
	//public string imagePath = Application.dataPath.Substring(0, Application.dataPath.Length - 4) + "Documents/" + "screenShot" + ".png";	
	
    public IEnumerator EncodeScreenShot() {
        // wait for graphics to render
        yield return new WaitForEndOfFrame();
 
         // create a texture to pass to encoding
		Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
 
        // put buffer into texture
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();
		
        // split the process up--ReadPixels() and the GetPixels() call inside of the encoder are both pretty heavy
        yield return 0;
 
        byte[] bytes = texture.EncodeToPNG();
 
        // save our test image (could also upload to WWW)
        File.WriteAllBytes(imagePath, bytes);

    }

}
