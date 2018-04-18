using System.Collections.Generic;
using UnityEngine;

public class Algorithms {
	/// <summary>
	/// 傳入一個陣列，打亂所有元素後回傳
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="array"></param>
	/// <returns></returns>
	public static T[] GetRandomArray<T>(T[] array) {
		T tmp;

		int halfN = array.Length / 2;
		int r;

		for (int i = 0; i < halfN; i++) {
			// 跟隨機一個互換位置
			r = Random.Range(0, array.Length);
			tmp = array[i];
			array[i] = array[r];
			array[r] = tmp;
		}

		return array;
	}
}
