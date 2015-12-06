using UnityEngine;
using System.Collections;

public class CapturePod : MonoBehaviour {
    public int Owner;
    public SpriteRenderer MapIcon;
    public SpriteRenderer CaptureMeter;
    public float CaptureRange;
    
    public float CaptureMax;
    public float CaptureAmount;
    public float CaptureSpeed;

    public Color Team1Color;
    public Color Team2Color;
    private LayerMask CaptureMask;
    private Material[] GlowMaterials;
    public int[] GlowMaterialIndices;

    void Start()
    {
        GameController myGameController = FindObjectOfType<GameController>();
        Team1Color = myGameController.GetTeam1PrimaryColor();
        Team2Color = myGameController.GetTeam2PrimaryColor();

        string[] layers = { "Player" };
        CaptureMask = LayerMask.GetMask(layers);
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        GlowMaterials = new Material[meshRenderers.Length];
        for(int i = 0; i < meshRenderers.Length; ++ i)
        {
            if(GlowMaterialIndices[i]==-1)
            {
                continue;
            }
            GlowMaterials[i] = meshRenderers[i].materials[GlowMaterialIndices[i]];
            GlowMaterials[i].SetColor("_EmissionColor", Color.grey);
        }
    }

    void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, CaptureRange, CaptureMask);
        bool team1Present = false;
        bool team2Present = false;
        int i = 0;
        while (i < hitColliders.Length)
        {
            if (hitColliders[i].tag == "Player1")
            {
                team1Present = true;
            }
            else if (hitColliders[i].tag == "Player2")
            {
                team2Present = true;
            }
            ++i;
        }
        if ((!team1Present && !team2Present) || (team1Present && team2Present))
        {
            CaptureMeter.enabled = false;
            return;
        }
        switch (Owner)
        {
            case 0: // No owner
                if (team1Present)
                {
                    Owner = 1;
                }
                else if (team2Present)
                {
                    Owner = 2;
                }
                CaptureAmount = Mathf.Min(CaptureMax, CaptureAmount + CaptureSpeed * Time.deltaTime);
                break;
            case 1: // Team1 Owner
                if (team1Present)
                {
                    CaptureAmount = Mathf.Min(CaptureMax, CaptureAmount + CaptureSpeed * Time.deltaTime);
                }
                else if (team2Present)
                {
                    CaptureAmount = Mathf.Min(CaptureMax, CaptureAmount - CaptureSpeed * Time.deltaTime);
                    if (CaptureAmount < 0f)
                    {
                        Owner = 2;
                    }
                }
                break;
            case 2: // Team2 Owner
                if (team1Present)
                {
                    CaptureAmount = Mathf.Min(CaptureMax, CaptureAmount - CaptureSpeed * Time.deltaTime);
                    if (CaptureAmount < 0f)
                    {
                        Owner = 1;
                    }
                }
                else if (team2Present)
                {
                    CaptureAmount = Mathf.Min(CaptureMax, CaptureAmount + CaptureSpeed * Time.deltaTime);
                }
                break;
        }
        if (CaptureAmount >= CaptureMax)
        {
            CaptureMeter.enabled = false;
            if (Owner == 1)
            {
                MapIcon.color = Team1Color;
                for (int j = 0; j < GlowMaterials.Length; ++j)
                {
                    if (GlowMaterialIndices[j] == -1)
                    {
                        continue;
                    }
                    GlowMaterials[j].SetColor("_EmissionColor", Team1Color * 50f);
                }
            }
            else
            {
                MapIcon.color = Team1Color;
                for (int j = 0; j < GlowMaterials.Length; ++j)
                {
                    if (GlowMaterialIndices[j] == -1)
                    {
                        continue;
                    }
                    GlowMaterials[j].SetColor("_EmissionColor", Team2Color * 50f);
                }
            }
        }
        else
        {
            CaptureMeter.enabled = true;
            if (Owner == 1)
            {
                CaptureMeter.color = Team1Color;
                MapIcon.color = Color.white;
            }
            else
            {
                CaptureMeter.color = Team2Color;
                MapIcon.color = Color.white;
            }
            CaptureMeter.transform.localScale = new Vector3(CaptureAmount / CaptureMax * 10, 1f, 1f);
        }
    }
}
