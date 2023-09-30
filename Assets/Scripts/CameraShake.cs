using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField]
    float duration, magnitude;
    public IEnumerator Shake()
    {
        Vector3 originalPosition = transform.localPosition;

        float elapsed = 0.0f;

        while(elapsed < duration)
        {
            Vector2 offset;
            offset.x = Random.Range(-1f, 1f) * magnitude + transform.localPosition.x;
            offset.y = Random.Range(-1f, 1f) * magnitude + transform.localPosition.y;

            transform.localPosition = new Vector3(offset.x, offset.y, originalPosition.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPosition;
    }
}
