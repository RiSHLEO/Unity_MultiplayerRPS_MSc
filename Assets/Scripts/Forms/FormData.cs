using UnityEngine;

[CreateAssetMenu(fileName = "FormData", menuName = "Scriptable Objects/FormData")]
public class FormData : ScriptableObject
{
    public string FormName;
    public float MoveSpeed;
    public Color FormColor;
}
