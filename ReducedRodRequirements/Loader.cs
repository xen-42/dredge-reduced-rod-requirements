using UnityEngine;

namespace ReducedRodRequirements
{
	public class Loader
	{
		/// <summary>
		/// This method is run by Winch to initialize your mod
		/// </summary>
		public static void Initialize()
		{
			var gameObject = new GameObject(nameof(ReducedRodRequirements));
			gameObject.AddComponent<ReducedRodRequirements>();
			GameObject.DontDestroyOnLoad(gameObject);
		}
	}
}