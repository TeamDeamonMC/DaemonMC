namespace DaemonMC.Utils.Game
{
    public class Animation
    {
        public string ControllerName { get; set; }
        public string AnimationName { get; set; }
        public string NextAnimationName { get; set; }

        public Animation(string controllerName, string animationName, string nextAnimationName)
        {
            ControllerName = controllerName;
            AnimationName = animationName;
            NextAnimationName = nextAnimationName;
        }
    }
}
