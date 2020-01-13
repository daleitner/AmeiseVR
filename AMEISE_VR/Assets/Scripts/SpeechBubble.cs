using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
	public class SpeechBubble : GameObjectModelBase
	{
		private readonly TextMeshPro _text1;
		private readonly TextMeshPro _text2;
		public SpeechBubble(GameObject gameObject) 
			: base(gameObject)
		{
			_text1 = GameObject.transform.Find("BubbleText").GetComponent<TextMeshPro>();
			_text2 = GameObject.transform.Find("BubbleText2").GetComponent<TextMeshPro>();
		}

		public void SetText(string text)
		{
			_text1.text = text;
			_text2.text = text;
		}
	}
}
