<<<<<<< HEAD
﻿using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour
{
	Vector3 angle;

	void Start()
	{
		angle = transform.eulerAngles;
	}

	void Update()
	{
		angle.y += Time.deltaTime * 100;
		transform.eulerAngles = angle;
	}

}
=======
﻿using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour
{
	Vector3 angle;

	void Start()
	{
		angle = transform.eulerAngles;
	}

	void Update()
	{
		angle.y += Time.deltaTime * 100;
		transform.eulerAngles = angle;
	}

}
>>>>>>> dda09d7b12b2ce1a2d8bd988e7c4a533b0029525
