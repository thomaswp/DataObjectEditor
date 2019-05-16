namespace Emigre.Data
{
    public class Constants
    {
        public const string GAME_NAME = "Emigré";

        public const string DIR = "";

        public const string DIR_IMAGES = DIR + "images/";
        public const string DIR_IMAGES_BG = DIR_IMAGES + "bg/";
        public const string DIR_IMAGES_MAPS = DIR_IMAGES + "maps/";
        public const string DIR_IMAGES_CHARACTERS = DIR_IMAGES + "characters/";
        public const string DIR_IMAGES_PORTRAITS = DIR_IMAGES + "portraits/";
        public const string DIR_IMAGES_ICONS = DIR_IMAGES + "icons/";
        public const string DIR_IMAGES_POSES = DIR_IMAGES + "poses/";

        public const string DIR_VIDEO = DIR + "../StreamingAssets/";

        public const string DIR_SOUND = DIR + "sound/";
        public const string DIR_SOUND_BG = DIR_SOUND + "bg/";
        public const string DIR_SOUND_SE = DIR_SOUND + "se/";

        public const string TITLE_FONT = "Cambria";
        public const string TEXT_FONT = "Georgia";

        public static string RemoveExtension(string path)
        {
            if (path == null) return null;
            if (path.Contains("."))
            {
                path = path.Substring(0, path.LastIndexOf("."));
            }
            return path;
        }
    }
}