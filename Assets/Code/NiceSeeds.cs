using UnityEngine;
using System.Collections;

public class NiceSeeds : MonoBehaviour {
    //x 174932 y 557417 //on top of mountain(snow) //mounts, lake
    //x 977685 y 513374 //on mountain(dark area) //valley, lake
    //x 242849 y 175532 //
    //x 173713 y 89290 //on top of mountain(snow)
    //x 533987 y 409793 //on top of huge snow top

    //x 348497 y 918720 //biome split
    //x 5299572 y 1126853 //test biome split
}

/*
// const SIZE = 16 * 1024 * 1024;
// array is an int[]
// list is a List<int>

1a. for (int i = 0; i < SIZE; i++) { x += array[i]; }
1b. for (int i = 0; i < SIZE; i++) { x += list[i]; }
2a. foreach (int val in array) { x += val; }
2b. foreach (int val in list) { x += val; }
 3. x = list.Sum(); // linq extension

                              time   memory
1a. for loop over array .... 35 ms .... 0 B
1b. for loop over list ..... 62 ms .... 0 B
2a. foreach over array ..... 35 ms .... 0 B
2b. foreach over list ..... 120 ms ... 24 B
 3. linq sum() ............ 271 ms ... 24 B
*/
