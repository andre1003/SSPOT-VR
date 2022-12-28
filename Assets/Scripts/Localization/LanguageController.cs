using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LanguageController : MonoBehaviour {
    public string language = "pt-BR";

    // Start is called before the first frame update
    void Start() {
        //Location();
    }

    // Update is called once per frame
    void Update() {
        if(Input.GetKeyDown(KeyCode.L))
            Location();
    }

    /// <summary>
    /// When player clicks on this object, the next level is loaded
    /// </summary>
    public void OnPointerClick() {
        Location();
    }

    public void Location() {
        Debug.Log("Changing language...");

        if(language == "pt-BR")
            language = "en";
        else
            language = "pt-BR";

        LocalizationSettings settings = LocalizationSettings.Instance;

        LocaleIdentifier localCode = new LocaleIdentifier(language);

        for(int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++) {
            Locale aLocale = LocalizationSettings.AvailableLocales.Locales[i];
            LocaleIdentifier anIdentifier = aLocale.Identifier;

            if(anIdentifier == localCode)
                LocalizationSettings.SelectedLocale = aLocale;
        }
    }
}
