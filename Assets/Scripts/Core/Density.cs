using UnityEngine;
using System.Collections;
using SimplexNoise;

public static class Density {

	static float Sphere(Vector3 worldPosition, Vector3 origin, float radius){
	    return (worldPosition - origin).magnitude - radius;
    }

    // ----------------------------------------------------------------------------

	static float Cuboid(Vector3 worldPosition, Vector3 origin, Vector3 halfDimensions){
        Vector3 local_pos = worldPosition - origin;
        Vector3 pos = local_pos;

        Vector3 d = pos - halfDimensions;
        float m = Mathf.Max(d.x, Mathf.Max(d.y, d.z));
        return Mathf.Min(m, (Vector3.Max(d, Vector3.zero).magnitude));
    }

    // ----------------------------------------------------------------------------

	static float FractalNoise(float noise, int octaves, float frequency){
		float n = 0;
		for (int i=1; i<=octaves; i*=2) {
			n = (noise*i)/(1/i);
		}
		return n;
    }

    // ----------------------------------------------------------------------------

   public static float DensityFunc(Vector3 worldPosition){

		float warpx = Noise.Generate (worldPosition.x * 0.0004f);
		float warpy = Noise.Generate (worldPosition.y * 0.0004f);
		float warpz = Noise.Generate (worldPosition.z * 0.0004f);
		Vector3 warp = new Vector3 (warpx,warpy, warpz);
		worldPosition += warp * 8;  
		float sphere = Sphere(worldPosition, new Vector3(0,0,0), 500);
		float noise = sphere;
		noise += (Noise.Generate(worldPosition*0.400f))*0.5f/2f;
		noise += (Noise.Generate(worldPosition*0.200f))*1/2f;
		noise += (Noise.Generate(worldPosition*0.100f))*2/2f;
		noise += (Noise.Generate(worldPosition*0.050f))*4/2f;
		noise += (Noise.Generate(worldPosition*0.025f))*8/2f;
		noise += (Noise.Generate(worldPosition*0.010f))*16/2f;

		noise = Saturate (noise, sphere, worldPosition, 498);

		return noise ;
    }

	public static float Saturate(float noise1, float noise2, Vector3 position, float height){
			float h = Vector3.Distance (Vector3.zero, position);
		if (h < height)
			return noise2;
		else
			return noise1;
	}
}
