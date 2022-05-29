<<<<<<< HEAD
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MultiKeyDictionary<T1, T2, T3> : Dictionary<T1, Dictionary<T2, T3>>
{
	new public Dictionary<T2, T3> this[T1 key]
	{
		get
		{
			if (!ContainsKey(key))
				Add(key, new Dictionary<T2, T3>());

			Dictionary<T2, T3> returnObj;
			TryGetValue(key, out returnObj);

			return returnObj;
		}
	}

	public bool ContainsKey(T1 key1, T2 key2)
	{
		Dictionary<T2, T3> returnObj;
		TryGetValue(key1, out returnObj);
		if (returnObj == null)
			return false;

		return returnObj.ContainsKey(key2);
	}
}
=======
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MultiKeyDictionary<T1, T2, T3> : Dictionary<T1, Dictionary<T2, T3>>
{
	new public Dictionary<T2, T3> this[T1 key]
	{
		get
		{
			if (!ContainsKey(key))
				Add(key, new Dictionary<T2, T3>());

			Dictionary<T2, T3> returnObj;
			TryGetValue(key, out returnObj);

			return returnObj;
		}
	}

	public bool ContainsKey(T1 key1, T2 key2)
	{
		Dictionary<T2, T3> returnObj;
		TryGetValue(key1, out returnObj);
		if (returnObj == null)
			return false;

		return returnObj.ContainsKey(key2);
	}
}
>>>>>>> dda09d7b12b2ce1a2d8bd988e7c4a533b0029525
