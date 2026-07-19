using UnityEngine;

namespace KTG
{
    // Phase D3 (thu nghiem): camera phoi canh nghieng kieu diorama.
    // The gioi van la sprite 2D nam phang tren mat XY; prop/nhan vat duoc
    // "dung day" (nghieng cung goc camera, chan giu tren dat) de ca man hinh
    // nhin nhu mo hinh thu nho — dung cach cua Octopath Traveler.
    public static class Hd2dView
    {
        public const bool Diorama = true;   // false = quay ve ortho pixel-perfect cu
        public const float TiltDeg = 20f;   // goc nghieng camera quanh truc X
        public const float Fov = 40f;       // field of view phoi canh
        public const float Dist = 15f;      // khoang cach camera -> diem focus (~11 o theo chieu doc)

        public static readonly Quaternion Rot = Quaternion.Euler(-TiltDeg, 0f, 0f);

        // Dung sprite day theo goc camera (sprite pivot-bottom nen chan van cham dat).
        public static void StandUp(Transform t)
        {
            if (Diorama) t.localRotation = Rot;
        }
    }
}
