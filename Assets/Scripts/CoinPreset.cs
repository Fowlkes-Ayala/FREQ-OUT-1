using UnityEngine;

[CreateAssetMenu(fileName = "CoinPreset", menuName = "CoinPreset")]
public class CoinPreset : ScriptableObject
{
    [Header("Array index = beat")]
    [Header("Index 0 is the beat closest to the player")]
    [Header("Value = lane")]
    [Header("Value of -1 means there is no coin on this beat")]
    public int[] coinLanes;

    private void OnValidate()
    {
        if (coinLanes == null || coinLanes.Length != 4)
        {
            var newArray = new int[4];
            if (coinLanes != null)
            {
                int copyCount = Mathf.Min(coinLanes.Length, newArray.Length);
                System.Array.Copy(coinLanes, newArray, copyCount);
            }
            coinLanes = newArray;
        }
    }
}
