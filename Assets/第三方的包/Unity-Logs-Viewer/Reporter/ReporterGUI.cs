<<<<<<< HEAD
﻿using UnityEngine;
using System.Collections;

public class ReporterGUI : MonoBehaviour
{
	Reporter reporter;
	void Awake()
	{
		reporter = gameObject.GetComponent<Reporter>();
	}

	void OnGUI()
	{
		reporter.OnGUIDraw();
	}
}
=======
﻿using UnityEngine;
using System.Collections;

public class ReporterGUI : MonoBehaviour
{
	Reporter reporter;
	void Awake()
	{
		reporter = gameObject.GetComponent<Reporter>();
	}

	void OnGUI()
	{
		reporter.OnGUIDraw();
	}
}
>>>>>>> dda09d7b12b2ce1a2d8bd988e7c4a533b0029525
