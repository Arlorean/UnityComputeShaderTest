# Unity Compute Shader Realtime Ray Tracing

This is a Unity/HLSL port of the excellent [Annotated Realtime Raytracing](https://www.tinycranes.com/blog/2015/05/annotated-realtime-raytracing/) blog post by [Kevin Fung](https://www.tinycranes.com/).

The original GLSL shader runs live in a web browser on [ShaderToy](https://www.shadertoy.com/view/4ljGRd). Unity looks the same but at 138fps (Intel HD GPU).

![Unity Editor Screenshot](https://github.com/Arlorean/UnityComputeShaderTest/raw/master/example.png)

## Porting GLSL to HLSL

This was much easier than I thought it would be. Here are the changes I made:
* Added HLSL kernel (`CSMain`) that calls the GLSL fragment shader (`mainImage`)
* Added `iGlobalTime` and `iResolution` shader variables
* Replaced `vec2` with `float2`
* Replaced `vec3` with `float3`
* Replaced `vec4` with `float4`
* Expand constructors with 1 argument to n, e.g. `vec3(1.0)` -> `float3(1.0,1.0,1.0)`
* Change `const` to `static const`
* Change struct initializer syntax, e.g. `Material(vec3(0.0,0.2,1.0),1.0,0.0)` -> `{ float3(0.0,0.2,1.0),1.0,0.0 }`

## Unity Compute Shaders

I learnt about Unity Compute Shaders from [Coxlin's Blog](http://www.lindsaygcox.co.uk/tutorials/unity-shader-tutorial-an-intro-to-compute-shaders/). This article just multiplies 4 integers by 2.0 in a Computer Shader and prints the results to the console. Everything you need to know to get started.

Understanding the relationship between the `[numthreads(x,y,z)]` attribute in the shader, and the `shader.Dispatch(kernelIndex, gx,gy,gz)` call in C#, is fundamental. The kernel `CSMain` is executed `(x*y*z) * (gx*gy*gz)` times and the `id` passed into the kernel ranges from `(0,0,0)` to `(x*gx, y*gy, z*gz)`.

Here is how the kernel is evaluated for each thread in pseudocode:

```
        const int numThreadsX, numThreadsY, numThreadsZ;

        static void kernel(int x, int y, int z) { }

        static void Dispatch(int threadGroupsX, int threadGroupsY, int threadGroupsZ) {
            for (var gx = 0; gx < threadGroupsX; gx++) {
                for (var gy = 0; gy < threadGroupsY; gy++) {
                    for (var gz = 0; gz < threadGroupsZ; gz++) {

                        // Dispatch group
                        for (var x = 0; x < numThreadsX; x++) {
                            for (var y = 0; y < numThreadsY; y++) {
                                for (var z = 0; z < numThreadsZ; z++) {
                                    kernel(
                                        gx * numThreadsX + x,
                                        gy * numThreadsY + y,
                                        gz * numThreadsZ + z
                                    );
                                }
                            }
                        }
                    }
                }
            }
        }
```
