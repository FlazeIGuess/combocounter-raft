/*
 * ComboCounter - Combat combo tracking mod for Raft
 * Copyright (C) 2024 Flaze
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published
 * by the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU Affero General Public License for more details.
 * 
 * You should have received a copy of the GNU Affero General Public License
 * along with this program. If not, see <https://www.gnu.org/licenses/>.
 */

using RaftModLoader;
using UnityEngine;
using UnityEngine.UI;
using HMLLibrary;
using HarmonyLib;
using System.Runtime.CompilerServices;

public class ComboCounter : Mod
{
    // Singleton Instance (for access from static Harmony patches)
    public static ComboCounter Instance { get; private set; }
    
    // Harmony instance
    private Harmony harmony;
    
    // UI Components
    private Canvas comboCanvas;
    private GameObject comboCounterText;
    private Text comboTextComponent;
    private Coroutine comboBlinkCoroutine;
    
    // Configuration (can be changed via Extra Settings API)
    private float comboResetTime = 5f;
    private bool showComboCounter = true;
    private float comboColorR = 0.95f;
    private float comboColorG = 0.89f;
    private float comboColorB = 0.77f;
    
    // Combo state
    private int comboCount = 0;
    private int killCount = 0;
    private float lastHitTime = 0f;
    
    // Extra Settings API integration
    static bool ExtraSettingsAPI_Loaded = false;

    public void Start()
    {
        Instance = this;
        
        try
        {
            harmony = new Harmony("com.combocounter.mod");
            harmony.PatchAll(typeof(ComboCounter).Assembly);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[ComboCounter] ERROR applying Harmony patches: {ex.Message}");
        }
        
        CreateComboCounterUI();
    }
    
    public void Update()
    {
        // Check if combo should reset
        if (comboCount > 0 && Time.time - lastHitTime > comboResetTime)
        {
            ResetCombo();
        }
        
        // Check if combo is about to expire and start blinking
        if (comboCount > 0 && showComboCounter)
        {
            float timeRemaining = comboResetTime - (Time.time - lastHitTime);
            float blinkThreshold = comboResetTime * 0.3f;
            
            if (timeRemaining <= blinkThreshold && comboBlinkCoroutine == null)
            {
                comboBlinkCoroutine = StartCoroutine(BlinkComboCounter());
            }
        }
    }
    
    public void ExtraSettingsAPI_Load()
    {
        ExtraSettingsAPI_Loaded = true;
    }
    
    public void ExtraSettingsAPI_SettingsOpen()
    {
        // Load current settings when menu opens
        LoadSettingsFromAPI();
    }
    
    public void ExtraSettingsAPI_SettingsClose()
    {
        try
        {
            LoadSettingsFromAPI();
            UpdateComboVisibility();
            UpdateComboColor();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[ComboCounter] ERROR in SettingsClose: {ex.Message}");
        }
    }
    
    private void LoadSettingsFromAPI()
    {
        if (!ExtraSettingsAPI_Loaded) return;
        
        try
        {
            comboResetTime = GetFloatFromAPI("comboResetTime", comboResetTime);
            showComboCounter = GetBoolFromAPI("showComboCounter", showComboCounter);
            comboColorR = GetFloatFromAPI("comboColorR", comboColorR);
            comboColorG = GetFloatFromAPI("comboColorG", comboColorG);
            comboColorB = GetFloatFromAPI("comboColorB", comboColorB);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[ComboCounter] ERROR loading settings: {ex.Message}");
        }
    }
    
    private void UpdateComboColor()
    {
        if (comboTextComponent != null)
        {
            Color comboColor = new Color(comboColorR, comboColorG, comboColorB, comboTextComponent.color.a);
            comboTextComponent.color = comboColor;
        }
    }
    
    private void UpdateComboVisibility()
    {
        if (comboCounterText != null)
        {
            comboCounterText.SetActive(showComboCounter && comboCount > 0);
        }
    }
    
    public void IncrementCombo(bool isKill = false)
    {
        comboCount++;
        lastHitTime = Time.time;
        
        if (isKill)
        {
            killCount++;
        }
        
        // Stop blinking when combo is extended
        if (comboBlinkCoroutine != null)
        {
            StopCoroutine(comboBlinkCoroutine);
            comboBlinkCoroutine = null;
            
            // Reset alpha to full
            if (comboTextComponent != null)
            {
                Color color = comboTextComponent.color;
                color.a = 1f;
                comboTextComponent.color = color;
            }
        }
        
        if (showComboCounter)
        {
            UpdateComboDisplay();
        }
    }
    
    private void ResetCombo()
    {
        comboCount = 0;
        killCount = 0;
        
        if (comboBlinkCoroutine != null)
        {
            StopCoroutine(comboBlinkCoroutine);
            comboBlinkCoroutine = null;
        }
        
        if (comboCounterText != null)
        {
            comboCounterText.SetActive(false);
        }
    }
    
    private void UpdateComboDisplay()
    {
        if (comboTextComponent != null && comboCounterText != null)
        {
            comboCounterText.SetActive(true);
            
            if (comboCount == 1)
            {
                if (killCount > 0)
                {
                    comboTextComponent.text = "KILLED!";
                }
                else
                {
                    comboTextComponent.text = "HIT!";
                }
            }
            else
            {
                string killText = killCount > 0 ? $" - {killCount} KILLED!" : "";
                comboTextComponent.text = $"{comboCount}x COMBO!{killText}";
            }
        }
    }
    
    private System.Collections.IEnumerator BlinkComboCounter()
    {
        if (comboTextComponent == null)
        {
            yield break;
        }
        
        while (comboCount > 0)
        {
            // Fade out
            for (float t = 0; t < 0.3f; t += Time.deltaTime)
            {
                if (comboTextComponent == null) yield break;
                
                Color color = comboTextComponent.color;
                color.a = Mathf.Lerp(1f, 0.2f, t / 0.3f);
                comboTextComponent.color = color;
                yield return null;
            }
            
            // Fade in
            for (float t = 0; t < 0.3f; t += Time.deltaTime)
            {
                if (comboTextComponent == null) yield break;
                
                Color color = comboTextComponent.color;
                color.a = Mathf.Lerp(0.2f, 1f, t / 0.3f);
                comboTextComponent.color = color;
                yield return null;
            }
        }
        
        comboBlinkCoroutine = null;
    }
    
    [MethodImpl(MethodImplOptions.NoInlining)]
    public float GetFloatFromAPI(string SettingName, float DefaultValue) { return DefaultValue; }
    
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool GetBoolFromAPI(string SettingName, bool DefaultValue) { return DefaultValue; }
    
    public void OnModUnload()
    {
        if (comboCanvas != null)
        {
            Destroy(comboCanvas.gameObject);
        }
        
        if (harmony != null)
        {
            harmony.UnpatchAll("com.combocounter.mod");
        }
        
        Instance = null;
    }

    private void CreateComboCounterUI()
    {
        try
        {
            GameObject canvasObject = new GameObject("ComboCounterCanvas");
            comboCanvas = canvasObject.AddComponent<Canvas>();
            comboCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            comboCanvas.sortingOrder = 1;
            
            CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);
            
            canvasObject.AddComponent<GraphicRaycaster>();
            DontDestroyOnLoad(canvasObject);
            
            comboCounterText = new GameObject("ComboCounterText");
            comboCounterText.transform.SetParent(comboCanvas.transform, false);
            
            comboTextComponent = comboCounterText.AddComponent<Text>();
            
            // Try to find Raft's UI font
            Font raftFont = null;
            try
            {
                Text[] allTexts = Resources.FindObjectsOfTypeAll<Text>();
                foreach (Text text in allTexts)
                {
                    if (text.font != null && text.font.name != "Arial")
                    {
                        raftFont = text.font;
                        break;
                    }
                }
                
                if (raftFont == null)
                {
                    Font[] allFonts = Resources.FindObjectsOfTypeAll<Font>();
                    foreach (Font font in allFonts)
                    {
                        string fontName = font.name.ToLower();
                        if (fontName.Contains("chinese") || fontName.Contains("rock") || 
                            fontName.Contains("raft") || fontName.Contains("game"))
                        {
                            raftFont = font;
                            break;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[ComboCounter] Could not search for Raft fonts: {ex.Message}");
            }
            
            if (raftFont != null)
            {
                comboTextComponent.font = raftFont;
            }
            else
            {
                comboTextComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }
            
            comboTextComponent.fontSize = 32;
            comboTextComponent.alignment = TextAnchor.MiddleCenter;
            comboTextComponent.color = new Color(comboColorR, comboColorG, comboColorB, 1f);
            comboTextComponent.text = "";
            
            // Add outline for better visibility
            Outline outline = comboCounterText.AddComponent<Outline>();
            outline.effectColor = new Color(0f, 0f, 0f, 0.8f);
            outline.effectDistance = new Vector2(2, -2);
            
            RectTransform comboRect = comboCounterText.GetComponent<RectTransform>();
            comboRect.anchorMin = new Vector2(0.5f, 0.5f);
            comboRect.anchorMax = new Vector2(0.5f, 0.5f);
            comboRect.pivot = new Vector2(0.5f, 0.5f);
            comboRect.anchoredPosition = new Vector2(0, -80);
            comboRect.sizeDelta = new Vector2(400, 50);
            
            comboCounterText.SetActive(false);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[ComboCounter] ERROR during combo UI creation: {ex.Message}");
        }
    }
}

// ============================================================================
// HARMONY PATCHES
// ============================================================================

[HarmonyPatch(typeof(Network_Host), "DamageEntity")]
public class HarmonyPatch_NetworkHost_DamageEntity
{
    private static float healthBeforeDamage = -1f;
    
    [HarmonyPrefix]
    public static void Prefix(
        Network_Entity entity,
        float damage,
        EntityType damageInflictorEntityType)
    {
        try
        {
            healthBeforeDamage = -1f;
            
            if (damage <= 0f || damageInflictorEntityType != EntityType.Player)
            {
                return;
            }
            
            if (entity != null)
            {
                var stats = entity.GetComponent<Stat_Health>();
                if (stats != null)
                {
                    healthBeforeDamage = stats.Value;
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[ComboCounter] ERROR in DamageEntity_Prefix: {ex.Message}");
        }
    }
    
    [HarmonyPostfix]
    public static void Postfix(
        Network_Entity entity,
        Transform hitTransform,
        float damage,
        Vector3 hitPoint,
        Vector3 hitNormal,
        EntityType damageInflictorEntityType)
    {
        try
        {
            if (damage <= 0f || damageInflictorEntityType != EntityType.Player)
            {
                return;
            }
            
            if (entity != null)
            {
                var stats = entity.GetComponent<Stat_Health>();
                if (stats != null)
                {
                    // Only increment combo if entity was alive before the hit
                    if (healthBeforeDamage > 0f)
                    {
                        // Check if this hit killed the entity
                        bool isDeadNow = stats.Value <= 0f;
                        
                        if (ComboCounter.Instance != null)
                        {
                            ComboCounter.Instance.IncrementCombo(isDeadNow);
                        }
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[ComboCounter] ERROR in DamageEntity_Postfix: {ex.Message}");
        }
    }
}
