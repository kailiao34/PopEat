using System;
using UnityEngine;
using GoogleMobileAds.Api;

public class Admob : MonoBehaviour {

    public static BannerView bannerView;

    public void Start()
    {
#if UNITY_ANDROID
		string appId = "ca-app-pub-2763403976639382~3279308884"; //這是 PopEat Android 的 appId
																 //string appId = "ca-app-pub-3940256099942544~3347511713"; //這是測試用的 appID
#elif UNITY_IPHONE
            //尚未添加  註： Google Play 和 Apple 用的是不同 ID!!!
            string appId = "not_yet_added";
#else
            string appId = "unexpected_platform";
#endif

		// Initialize the Google Mobile Ads SDK.
		MobileAds.Initialize(appId);

        RequestBanner();
        bannerView.Show();
    }

    public void RequestBanner()
    {
#if UNITY_ANDROID
		string adUnitId = "ca-app-pub-2763403976639382/9852456090"; //這是 Banner1 的 adUnitId
																	//string adUnitId = "ca-app-pub-3940256099942544/6300978111"; //這是測試用的 adUnitId
#elif UNITY_IPHONE
            //尚未添加  註： Google Play 和 Apple 用的是不同 ID!!!    
            string adUnitId = "not_yet_added";
#else
            string adUnitId = "unexpected_platform";
#endif

		// Create a 320x50 banner at the top of the screen.
		// bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Top);
		bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Top);

        //Banner behaviour 參數
        // Called when an ad request has successfully loaded.
        bannerView.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        bannerView.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is clicked.
        bannerView.OnAdOpening += HandleOnAdOpened;
        // Called when the user returned from the app after an ad click.
        bannerView.OnAdClosed += HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        bannerView.OnAdLeavingApplication += HandleOnAdLeftApplication;

        // Load a banner ad.
        bannerView.LoadAd(createAdRequest());
    }

    public AdRequest createAdRequest()
    {
        return new AdRequest.Builder()
			//.AddTestDevice(AdRequest.TestDeviceSimulator)
			.AddTestDevice("5411A5C35D8EF6DDDE88C40E1008C3EE")		// 黑米的
			.AddTestDevice("2F74EAE94C43115E2BA1C40842852396")		// 不是黑米的
			.TagForChildDirectedTreatment(false)
            .Build();
    }

    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received");
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("HandleFailedToReceiveAd event received with message: "
                            + args.Message);
    }

    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpened event received");
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");
    }

    public void HandleOnAdLeftApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLeftApplication event received");
    }
}
