using UnityEngine;

public class TacticalFormation : MonoBehaviour
{
    public enum Formation { ThreeTwo, TwoOneTwo, FourThreeThree, FourFourTwo }
    [SerializeField] private Formation formation = Formation.FourFourTwo;
    [SerializeField] private Transform[] playerSlots;

    public Formation Current => formation;

    public void SetFormation(Formation value)
    {
        formation = value;
        ApplySlots();
    }

    public Vector3 GetSlot(int index, Vector3 origin, Vector3 forward, float width = 18f, float length = 30f)
    {
        int row = index % 3;
        int column = index / 3;
        int count = formation == Formation.ThreeTwo ? 2 : formation == Formation.TwoOneTwo ? 2 : formation == Formation.FourFourTwo ? 4 : 3;
        float x = (row - (count - 1) * .5f) * (width / Mathf.Max(1, count));
        float z = length * (.5f - column * .28f);
        return origin + forward * z + Vector3.right * x;
    }

    private void ApplySlots()
    {
        if (playerSlots == null) return;
        for (int i = 0; i < playerSlots.Length; i++) if (playerSlots[i] != null) playerSlots[i].localPosition = GetSlot(i, Vector3.zero, Vector3.forward);
    }
}
