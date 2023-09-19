using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    // This script controls the UI elements of the game
    // The UI elements
    // textmesh pro text reference for the amount of vampires
    public TMPro.TextMeshProUGUI vampireText;
    // textmesh pro text reference for the amount of humans
    public TMPro.TextMeshProUGUI humanText;
    // textmesh pro text reference for the amount of hunters
    public TMPro.TextMeshProUGUI hunterText;
    // reference to the agent spawner
    public AgentSpawner agentSpawner;
    // Start is called before the first frame update
    void Start()
    {
        // Set the text to the starting values
        vampireText.text = "Vampires: " + agentSpawner.GetNumVampires();
        humanText.text = "Humans: " + agentSpawner.GetNumCivvies();
        hunterText.text = "Hunters: " + agentSpawner.GetNumHunters();
    }

    // Update is called once per frame
    void Update()
    {
        // Once every second, update all UI elements
        if (Time.frameCount % 60 == 0)
        {
            // Update the amount of vampires
            vampireText.text = "Vampires: " + agentSpawner.GetVampires().Count;
            // Update the amount of humans
            humanText.text = "Humans: " + agentSpawner.GetCivilians().Count;
            // Update the amount of hunters
            hunterText.text = "Hunters: " + agentSpawner.GetHunters().Count;
        }
    }
}
