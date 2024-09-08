using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class WeatherController : MonoBehaviour
{
    [SerializeField] private Light _directionalLight;
    [SerializeField] private RectTransform _cloudyObject;
    [SerializeField] private RectTransform _sunnyObject;
    private WeatherType _weatherType;
    private CancellationTokenSource cts;
    enum WeatherType
    {
        Cloudy,Sunny
    }

    [Button("WeatherChange")]
    public void OnWeatherToggle()
    {
        cts?.Cancel();
        cts = new CancellationTokenSource();
        switch (_weatherType)
        {
            case WeatherType.Sunny:
            {
                _weatherType = WeatherType.Cloudy;
                _cloudyObject.DOAnchorPos(new Vector2(0, 0), 0.5f).WithCancellation(cts.Token);
                _sunnyObject.DOAnchorPos(new Vector2(100, 0), 0.5f).WithCancellation(cts.Token);
                
                _directionalLight.colorTemperature = 8000;
                _directionalLight.transform.
                    DOLocalRotate(new Vector3(50,225,0), 1f)
                    .WithCancellation(cts.Token);
                break;
            }
            case WeatherType.Cloudy:
            {
                _weatherType = WeatherType.Sunny;
                _cloudyObject.DOAnchorPos(new Vector2(-100, 0), 0.5f).WithCancellation(cts.Token);
                _sunnyObject.DOAnchorPos(new Vector2(0, 0), 0.5f).WithCancellation(cts.Token);
                
                _directionalLight.colorTemperature = 3500;
                _directionalLight.transform.
                    DOLocalRotate(new Vector3(35,225,0), 1f)
                    .WithCancellation(cts.Token);
                break;
            }
        }
    }
}
