using System.Collections;

using UnityEngine;

public class AlphaTest : MonoBehaviour
{
    [Range (0.1f, 1.0f)]
     public float fadeSpeed = 1f;    // How fast alpha value decreases.
     public Color fadeColor = new Color (0, 0, 0, 0);
 
     private Material m_Material;    // Used to store material reference.
     private Color m_Color;            // Used to store color reference.
 
     void Start ()
     {
         // Get reference to object's material.
         m_Material = GetComponent <Renderer> ().material;
 
         // Get material's starting color value.
         m_Color = m_Material.color;
 
         // Must use "StartCoroutine()" to execute 
         // methods with return type of "IEnumerator".
         StartCoroutine (AlphaFade ());
     }
 
     // This method fades only the alpha.
     IEnumerator AlphaFade ()
     {
         // Alpha start value.
         float alpha = 1.0f;
 
         // Loop until aplha is below zero (completely invisalbe)
         while (alpha > 0.0f)
         {
             // Reduce alpha by fadeSpeed amount.
             alpha -= fadeSpeed * Time.deltaTime;
 
             // Create a new color using original color RGB values combined
             // with new alpha value. We have to do this because we can't 
             // change the alpha value of the original color directly.
             m_Material.color = new Color (m_Color.r, m_Color.g, m_Color.b, alpha);
 
             yield return null;
         }
     }
 
 
     // This method fades from original color to "fadeColor"
     IEnumerator ColorFade ()
     {
         // Lerp start value.
         float change = 0.0f;
         
         // Loop until lerp value is 1 (fully changed)
         while (change < 1.0f)
         {
             // Reduce change value by fadeSpeed amount.
             change += fadeSpeed * Time.deltaTime;
 
             m_Material.color = Color.Lerp (m_Color, fadeColor, change);
             
             yield return null;
         }
     }
}