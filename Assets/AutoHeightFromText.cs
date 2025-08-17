using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(LayoutElement))]
public class AutoHeightFromText : MonoBehaviour
{
	public TextMeshProUGUI text;
	public float padding = 10f;

	void Update()
	{
		if (text != null)
		{
			float preferredHeight = text.preferredHeight + padding;
			GetComponent<LayoutElement>().preferredHeight = preferredHeight;
		}
	}
}
