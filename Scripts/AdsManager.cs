using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance;

    [Header("App IDs — thay bằng ID thật khi publish")]
    public string androidAppId     = "ca-app-pub-3940256099942544~3347511713"; // test
    
    [Header("Ad Unit IDs")]
    public string interstitialId   = "ca-app-pub-3940256099942544/1033173712"; // test
    public string rewardedId       = "ca-app-pub-3940256099942544/5224354917"; // test

    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;
    private Action rewardCallback;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        MobileAds.Initialize(status =>
        {
            Debug.Log("AdMob initialized");
            LoadInterstitial();
            LoadRewarded();
        });
    }

    // ── Interstitial ──────────────────────────────

    void LoadInterstitial()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        var request = new AdRequest();
        InterstitialAd.Load(interstitialId, request, (ad, error) =>
        {
            if (error != null) { Debug.LogError("Interstitial load failed: " + error); return; }
            interstitialAd = ad;
            interstitialAd.OnAdFullScreenContentClosed += LoadInterstitial;
        });
    }

    public void ShowInterstitial()
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
            interstitialAd.Show();
        else
            Debug.Log("Interstitial not ready");
    }

    // ── Rewarded ──────────────────────────────────

    void LoadRewarded()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        var request = new AdRequest();
        RewardedAd.Load(rewardedId, request, (ad, error) =>
        {
            if (error != null) { Debug.LogError("Rewarded load failed: " + error); return; }
            rewardedAd = ad;
        });
    }

    public void ShowRewarded(Action onSuccess)
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardCallback = onSuccess;
            rewardedAd.Show(reward =>
            {
                rewardCallback?.Invoke();
                LoadRewarded();
            });
        }
        else
        {
            Debug.Log("Rewarded not ready");
        }
    }
}