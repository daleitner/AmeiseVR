using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class AvatarCollectionScript : MonoBehaviour
{
	public GameObject DefaultAvatar;

	public GameObject Axel;
	public GameObject Bernd;
	public GameObject Christine;
	public GameObject Diana;
	public GameObject Richard;
	public GameObject Stefanie;
	public GameObject Thomas;

	private readonly List<string> _names = new List<string>
	{
		nameof(Axel),
		nameof(Bernd),
		nameof(Christine),
		nameof(Diana),
		nameof(Richard),
		nameof(Stefanie),
		nameof(Thomas)
	};
    // Start is called before the first frame update
    void Start()
    {
	    var avatars = new List<GameObject>
	    {
		    Axel,
		    Bernd,
		    Christine,
		    Diana,
		    Richard,
		    Stefanie,
		    Thomas
	    };

		GameObjectCollection.AddGameObject(DefaultAvatar, GameObjectEnum.Avatar);

		for (int i = 0; i < avatars.Count; i++)
		{
			avatars[i].tag = "Avatar";
			GameObjectCollection.AddAvatar(avatars[i], _names[i]);
		}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
