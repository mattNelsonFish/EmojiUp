using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Text;

public class EmojiSelect : MonoBehaviour {

    Text testingText;

    void Awake() {
        testingText = gameObject.GetComponent<Text>();
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        testingText.text = GetEmoji().ToString();
	}

    public string GetEmoji() {
        int point = 0x1F379;
        int offset = point - 0x10000;
        char lead = Convert.ToChar(0xd800 + (offset >> 10));
        char trail = Convert.ToChar(0xdc00 + (offset & 0x3ff));

        //testingMesh.text = lead.ToString();
        //char emoji = Convert.ToChar("U+1F575");

        byte[] byteStream = Encoding.UTF8.GetBytes("0x1F379");
        string asciiString = Convert.ToBase64String (byteStream);

       // byte[] byteStream = Convert.FromBase64String (asciiString);
       // string unicodeString = Encoding.UTF8.GetString (byteStream);

        return asciiString;
    }
}
