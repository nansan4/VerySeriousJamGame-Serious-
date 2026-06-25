using UnityEngine;
using System.Collections;

public class DeliveryBrazier : MonoBehaviour
{
    [SerializeField] private Light brazierLight;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float amplitideH = 0.25f;
    [SerializeField] private float amplitideV = 0.4f;
    [SerializeField] private Vector2 lightIntensityRange = new Vector2(12, 15);

    [SerializeField] private Color regularColor = Color.orange;
    [SerializeField] private Color validColor = Color.green;
    [SerializeField] private Color notValidColor = Color.red;

    private Vector3 _initPos;
    private Vector3 _newPos;
    private float _rand1;
    private float _rand2;

    public void FadeLightColor(string eventName)
    {
        switch(eventName)
        {
            case "StartRegular":
                StartCoroutine(FadeLightRoutine(regularColor, fadeDuration));
                break;
            case "StartValid":
                StartCoroutine(FadeLightRoutine(validColor, fadeDuration));
                break;
            case "StartNotValid":
                StartCoroutine(FadeLightRoutine(notValidColor, fadeDuration));
                break;
            default:
                Debug.LogWarning("Unknown event name for deliver brazier: " + eventName);
                break;
        }
    }

    private IEnumerator FadeLightRoutine(Color targetColor, float duration)
    {
        Color initialColor = brazierLight.color;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            brazierLight.color = Color.Lerp(initialColor, targetColor, elapsedTime / duration);
            yield return null;
        }
        brazierLight.color = targetColor;
    }

    private void Start()
    {
        _initPos = brazierLight.transform.localPosition;
        _rand1 = Random.Range(0f, 100f);
        _rand2 = Random.Range(0f, 100f);
    }

    private void FixedUpdate()
    {
        float x = (Mathf.PerlinNoise(Time.time * speed, _rand1) - 0.5f )  * amplitideH;
        float y = (Mathf.PerlinNoise(38f, Time.time * speed + 10f) - 0.5f )  * amplitideV;
        float z = (Mathf.PerlinNoise(Time.time * speed + 20f, _rand2) - 0.5f )  * amplitideH;

        _newPos = (new Vector3(x, y, z)) * 0.1f;
        brazierLight.transform.localPosition = _initPos + _newPos;



        brazierLight.intensity = (Mathf.PerlinNoise(Time.time * speed, -_rand2 * _rand1)).MapRange(0f, 1f, lightIntensityRange.x, lightIntensityRange.y);
    }
}
