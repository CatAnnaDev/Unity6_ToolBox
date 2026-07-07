namespace CatAnnaDev.Cameras
{
    public static class CameraShake
    {
        public static void Shake(float trauma)
        {
            if (CameraShaker.Active != null) CameraShaker.Active.AddTrauma(trauma);
        }

        public static bool HasShaker => CameraShaker.Active != null;
    }
}
