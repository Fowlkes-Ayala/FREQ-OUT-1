using UnityEngine;

namespace DefaultNamespace
{
    public enum CoinPattern
    {
        None,
        SingleCenter
    }
    
    [CreateAssetMenu(fileName = "Song Data", menuName = "Song", order = 0)]
    public class SongData : ScriptableObject
    {
        public int BeatsPerMeasure = 4;
        [SerializeField] public int BPM;
        [SerializeField, Min(0)] public int totalMeasures;

        [SerializeField] public CoinPattern[] coinPatterns;

        private void OnValidate()
        {
            if (totalMeasures < 0) totalMeasures = 0;
            if (coinPatterns == null || coinPatterns.Length != totalMeasures)
            {
                var newArray = new CoinPattern[totalMeasures];
                if (coinPatterns != null)
                {
                    int copyCount = Mathf.Min(coinPatterns.Length, newArray.Length);
                    System.Array.Copy(coinPatterns, newArray, copyCount);
                }
                coinPatterns = newArray;
            }
        }
    }
}