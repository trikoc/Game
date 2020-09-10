using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Globals
{
    #region Static Constants

    public static class PlayerInput
    {
        public static string Horizontal     = "Horizontal";
        public static string Vertical       = "Vertical";
        public static string Fire1          = "Fire1";
        public static string Fire2          = "Fire2";
        public static string Fire3          = "Fire3";
        public static string Jump           = "Jump";
        public static string MouseX         = "Mouse X";
        public static string MouseY         = "Mouse Y";
        public static string Scroll         = "Mouse ScrollWheel";
        public static string Submit         = "Submit";
        public static string Cancel         = "Cancel";
        public static string Crouch         = "Crouch";
        public static string Run            = "Run";
        public static string Walk           = "Walk";
        public static string Rifle          = "Rifle";
        public static string Pistol         = "Pistol";
        public static string Knife          = "Knife";
        public static string UnlockCamera   = "Unlock Camera";
        public static string ChangeCamera   = "Change Camera";
    }


    public static class AnimatorCondition
    {
        public static string VelX               = "VelX";
        public static string VelY               = "VelY";
        public static string Grounded           = "Grounded";
        public static string Speed              = "Speed";
        public static string Direction          = "Direction";
        public static string GrabRifleTrigger   = "GrabRifle";
        public static string GrabPistolTrigger  = "GrabPistol";
        public static string GrabKnifeTrigger   = "GrabKnife";
        public static string PutAwayTrigger     = "PutAway(rifle,pistol,knife)";
        public static string Walk               = "Walk";
        public static string Crouch             = "Crouch";
        public static string AirVelocity        = "AirVelocity";
    }

    public static class AnimatorLayerIndex
    {

        public static int BaseLayer = 0;
        public static int UpperLayer = 1;
        public static int LowerLayer = 2;

    }

    #endregion

}

