using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.Audio;

public class SetMixerVolume : MonoBehaviour
{
	public AudioMixer MasterMixer;
	
	public void setMasterLevel(float audioLevel)
	{
		if (audioLevel > -29.9f)
		{
			MasterMixer.SetFloat("MasterVol", audioLevel);
		}
		else
		{
			MasterMixer.SetFloat("MasterVol", -80f);
		}
	}
	public void setMusicLevel(float audioLevel)
	{
		if (audioLevel > -29.9f)
		{
			MasterMixer.SetFloat("MasterVol", audioLevel);
		}
		else
		{
			MasterMixer.SetFloat("MasterVol", -80f);
		}
	}
	public void setSFXLevel(float audioLevel)
	{
		if (audioLevel > -29.9f)
		{
			MasterMixer.SetFloat("MasterVol", audioLevel);
		}
		else
		{
			MasterMixer.SetFloat("MasterVol", -80f);
		}
	}
}
