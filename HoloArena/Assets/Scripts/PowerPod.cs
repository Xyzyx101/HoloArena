using UnityEngine;
using System.Collections;

public class PowerPod : MonoBehaviour
{
    public int Owner;
    public SpriteRenderer MapIcon;
    public SpriteRenderer CaptureMeter;
    public float CaptureRange;

    public float PowerAmount;

    public float CaptureMax;
    public float CapturePercent;
    public float CaptureSpeed;

    public Color Team1Color;
    public Color Team2Color;
    private LayerMask CaptureMask;
    public Material GlowMaterial;

    void Start()
    {
        string[] layers = { "Player" };
        CaptureMask = LayerMask.GetMask(layers);
        MeshRenderer meshRenderer = GetComponentInChildren<MeshRenderer>();
        GlowMaterial = meshRenderer.materials[1];
        GlowMaterial.SetColor("_EmissionColor", Color.grey);
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
                CapturePercent = Mathf.Min(CaptureMax, CapturePercent + CaptureSpeed * Time.deltaTime);
                break;
            case 1: // Team1 Owner
                if (team1Present)
                {
                    CapturePercent = Mathf.Min(CaptureMax, CapturePercent + CaptureSpeed * Time.deltaTime);
                }
                else if (team2Present)
                {
                    CapturePercent = Mathf.Min(CaptureMax, CapturePercent - CaptureSpeed * Time.deltaTime);
                    if (CapturePercent < 0f)
                    {
                        Owner = 2;
                    }
                }
                break;
            case 2: // Team2 Owner
                if (team1Present)
                {
                    CapturePercent = Mathf.Min(CaptureMax, CapturePercent - CaptureSpeed * Time.deltaTime);
                    if (CapturePercent < 0f)
                    {
                        Owner = 1;
                    }
                }
                else if (team2Present)
                {
                    CapturePercent = Mathf.Min(CaptureMax, CapturePercent + CaptureSpeed * Time.deltaTime);
                }
                break;
        }
        if (CapturePercent >= CaptureMax)
        {
            CaptureMeter.enabled = false;
            if (Owner == 1)
            {
                MapIcon.color = Team1Color;
                GlowMaterial.SetColor("_EmissionColor", Team1Color * 50f);
            }
            else
            {
                MapIcon.color = Team1Color;
                GlowMaterial.SetColor("_EmissionColor", Team2Color);
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
            CaptureMeter.transform.localScale = new Vector3(CapturePercent / CaptureMax * 10, 1f, 1f);
        }
    }
}
