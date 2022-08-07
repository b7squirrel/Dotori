using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    string soundName;
    public enum soundType
    {
        jump,
        land,
        run,
        captureSwing,
        hitRollSwing,
        hitRoll_01,
        hitRoll_02,
        dash,
        die
    }
    soundType playerSoundType;

    public void Play(soundType _soundType)
    {
        playerSoundType = _soundType;
        switch (playerSoundType)
        {
            case soundType.jump:
                soundName = "whoosh_02";
                break;
            case soundType.land:
                soundName = "whoosh_02";
                break;
            case soundType.run:
                soundName = "whoosh_02";
                break;
            case soundType.captureSwing:
                soundName = "PanCapture";
                break;
            case soundType.hitRollSwing:
                soundName = "whoosh_01";
                break;
            case soundType.hitRoll_01:
                soundName = "pan_hit_03";
                break;
            case soundType.hitRoll_02:
                soundName = "fire_explosion_01";
                break;
            case soundType.dash:
                soundName = "whoosh_03";
                break;
            case soundType.die:
                soundName = "PlayerDie";
                break;
        }
        AudioManager.instance.Play(soundName);
    }
}
