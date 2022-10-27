using BubbleShooter;
using UnityEngine;


namespace BubbleShooter
{
    public struct OpenCellRequest
    {
        public Vector3 HitPosition;
        public ColorTypes PlayerColorTypes;
        public CellView HittedCellView;
    }
}