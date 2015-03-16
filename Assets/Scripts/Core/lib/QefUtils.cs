using UnityEngine;
using System.Collections;

namespace QefUtils{
	//-------------------------------------------------------------------------------
	//Smat3
	//-------------------------------------------------------------------------------
	public class SMat3{
		public float m00, m01, m02, m11, m12, m22;

		public SMat3(){
			this.Clear ();
		}

		public SMat3(float m00, float m01, float m02, float m11, float m12, float m22){
			this.SetSymmetric(m00, m01, m02, m11, m12, m22);
		}

		private SMat3(SMat3 rhs){
			this.SetSymmetric (rhs);
		}

		public void Clear(){
			this.SetSymmetric (0, 0, 0, 0, 0, 0);
		}

		public void SetSymmetric(float a00, float a01, float a02, float a11, float a12, float a22){
			this.m00 = a00;
			this.m01 = a01;
			this.m02 = a02;
			this.m11 = a11;
			this.m12 = a12;
			this.m22 = a22;
		}

		public void SetSymmetric(SMat3 rhs){
			this.SetSymmetric (rhs.m00, rhs.m01, rhs.m02, rhs.m11, rhs.m12, rhs.m22);
		}
	};

	//-------------------------------------------------------------------------------
	//Mat3
	//-------------------------------------------------------------------------------
	public class Mat3{
		public float m00, m01, m02, m10, m11, m12, m20, m21, m22;

		public Mat3(){
			this.Clear ();
		}

		public Mat3(float m00, float m01, float m02,
		            float m10, float m11, float m12,
		            float m20, float m21, float m22){
			this.Set (m00, m01, m02, m10, m11, m12, m20, m21, m22);
		}

		public Mat3(Mat3 rhs){
			this.Set (rhs);
		}

		public void Clear(){
			this.Set (0, 0, 0, 0, 0, 0, 0, 0, 0);
		}

		public void Set(float m00, float m01, float m02,
		                float m10, float m11, float m12,
		                float m20, float m21, float m22){
			this.m00 = m00;
			this.m01 = m01;
			this.m02 = m02;
			this.m10 = m10;
			this.m11 = m11;
			this.m12 = m12;
			this.m20 = m20;
			this.m21 = m21;
			this.m22 = m22;
		}

		public void Set(Mat3 rhs){
			this.Set (rhs.m00, rhs.m01, rhs.m02, rhs.m10, rhs.m11, rhs.m12, rhs.m20, rhs.m21, rhs.m22);
		}

		public void SetSymmetric(float a00, float a01, float a02,
		                         float a11, float a12, float a22){
			this.Set(a00, a01, a02, a01, a11, a12, a02, a12, a22);
		}

		public void SetSymmetric(SMat3 rhs){
			this.SetSymmetric(rhs.m00, rhs.m01, rhs.m02, rhs.m11, rhs.m12, rhs.m22);
		}
	};

	//-------------------------------------------------------------------------------
	//MatUtils
	//-------------------------------------------------------------------------------
	public static class MatUtils{

		public static float Fnorm(Mat3 a){
			return Mathf.Sqrt((a.m00 * a.m00) + (a.m01 * a.m01) + (a.m02 * a.m02)
			            + (a.m10 * a.m10) + (a.m11 * a.m11) + (a.m12 * a.m12)
			            + (a.m20 * a.m20) + (a.m21 * a.m21) + (a.m22 * a.m22));
		}

		public static float Fnorm(SMat3 a){
			return Mathf.Sqrt((a.m00 * a.m00) + (a.m01 * a.m01) + (a.m02 * a.m02)
			            + (a.m01 * a.m01) + (a.m11 * a.m11) + (a.m12 * a.m12)
			            + (a.m02 * a.m02) + (a.m12 * a.m12) + (a.m22 * a.m22));
		}

		public static float Off(Mat3 a){
			return Mathf.Sqrt((a.m01 * a.m01) + (a.m02 * a.m02) + (a.m10 * a.m10)
			            + (a.m12 * a.m12) + (a.m20 * a.m20) + (a.m21 * a.m21));
		}

		public static float Off(SMat3 a){
			return Mathf.Sqrt(2 * ((a.m01 * a.m01) + (a.m02 * a.m02) + (a.m12 * a.m12)));
		}

		public static void Mmul(Mat3 mout, Mat3 a, Mat3 b){
			mout.Set(a.m00 * b.m00 + a.m01 * b.m10 + a.m02 * b.m20,
			        a.m00 * b.m01 + a.m01 * b.m11 + a.m02 * b.m21,
			        a.m00 * b.m02 + a.m01 * b.m12 + a.m02 * b.m22,
			        a.m10 * b.m00 + a.m11 * b.m10 + a.m12 * b.m20,
			        a.m10 * b.m01 + a.m11 * b.m11 + a.m12 * b.m21,
			        a.m10 * b.m02 + a.m11 * b.m12 + a.m12 * b.m22,
			        a.m20 * b.m00 + a.m21 * b.m10 + a.m22 * b.m20,
			        a.m20 * b.m01 + a.m21 * b.m11 + a.m22 * b.m21,
			        a.m20 * b.m02 + a.m21 * b.m12 + a.m22 * b.m22);
		}

		public static void Mmul_ata(SMat3 mout, Mat3 a){
			mout.SetSymmetric(a.m00 * a.m00 + a.m10 * a.m10 + a.m20 * a.m20,
			                 a.m00 * a.m01 + a.m10 * a.m11 + a.m20 * a.m21,
			                 a.m00 * a.m02 + a.m10 * a.m12 + a.m20 * a.m22,
			                 a.m01 * a.m01 + a.m11 * a.m11 + a.m21 * a.m21,
			                 a.m01 * a.m02 + a.m11 * a.m12 + a.m21 * a.m22,
			                 a.m02 * a.m02 + a.m12 * a.m12 + a.m22 * a.m22);
		}

		public static void Transpose(Mat3 mout, Mat3 a)
		{
			mout.Set(a.m00, a.m10, a.m20, a.m01, a.m11, a.m21, a.m02, a.m12, a.m22);
		}

		public static Vector3 Vmul(Mat3 a, Vector3 v){
			float x = (a.m00 * v.x) + (a.m01 * v.y) + (a.m02 * v.z);
			float y = (a.m10 * v.x) + (a.m11 * v.y) + (a.m12 * v.z);
			float z = (a.m20 * v.x) + (a.m21 * v.y) + (a.m22 * v.z);
			return new Vector3 (x, y, z);
		}

		public static Vector3 Vmul_symmetric(SMat3 a, Vector3 v){
			float x = (a.m00 * v.x) + (a.m01 * v.y) + (a.m02 * v.z);
			float y = (a.m01 * v.x) + (a.m11 * v.y) + (a.m12 * v.z);
			float z = (a.m02 * v.x) + (a.m12 * v.y) + (a.m22 * v.z);
			return new Vector3(x, y, z);
		}
	};

	//-------------------------------------------------------------------------------
	//VecUtils
	//-------------------------------------------------------------------------------
	public static class VecUtils{

		public static void AddScaled(Vector3 v, float s, Vector3 rhs){
			float x = v.x + s * rhs.x;
			float y = v.y + s * rhs.y;
			float z = v.z + s * rhs.z;
			v.Set (x,y,z);
		}

		public static float Dot(Vector3 a, Vector3 b){
			return a.x * b.x + a.y * b.y + a.z * b.z;
		}

		public static Vector3 Normalize(Vector3 v){
			float len2 = Dot(v, v);
			if (Mathf.Abs(len2) < 1e-12) {
				v.Set(0, 0, 0);
				return v;
			} else {
				return Scale(v, 1 / Mathf.Sqrt(len2));
			}
		}
		
		public static Vector3 Sub(Vector3 a, Vector3 b){
			float v0 = a.x - b.x;
			float v1 = a.y - b.y;
			float v2 = a.z - b.z;
			return new Vector3 (v0, v1, v2);
		}

		public static Vector3 Scale(Vector3 v, float s){
			float v0 = v.x * s;
			float v1 = v.y * s;
			float v2 = v.z * s;
			return new Vector3(v0, v1, v2);
		}
	};

	//-------------------------------------------------------------------------------
	//Svd
	//-------------------------------------------------------------------------------
	public static class Svd {
		public static float SolveSymmetric(SMat3 A, Vector3 b, Vector3 x, float svd_tol, int svd_sweeps, float pinv_tol){
			Mat3 pinv = new Mat3 ();
			Mat3 V = new Mat3 ();
			SMat3 VTAV = new SMat3();
			GetSymmetricSvd(A, VTAV, V, svd_tol, svd_sweeps);
			Pseudoinverse(pinv, VTAV, V, pinv_tol);
			x = MatUtils.Vmul(pinv, b);
			return CalcError(A, x, b);
		}

		public static void GetSymmetricSvd(SMat3 a, SMat3 vtav, Mat3 v, float tol, int max_sweeps){
			vtav.SetSymmetric(a);
			v.Set(1, 0, 0, 0, 1, 0, 0, 0, 1);
			float delta = tol * MatUtils.Fnorm(vtav);
			for (int i = 0; i < max_sweeps && MatUtils.Off(vtav) > delta; ++i) {
				Rotate01(vtav, v);
				Rotate02(vtav, v);
				Rotate12(vtav, v);
			}
		}

		public static void Pseudoinverse(Mat3 mout, SMat3 d, Mat3 v, float tol){
			float d0 = pinv(d.m00, tol), d1 = pinv(d.m11, tol), d2 = pinv(d.m22,tol);
			mout.Set(v.m00 * d0 * v.m00 + v.m01 * d1 * v.m01 + v.m02 * d2 * v.m02,
			        v.m00 * d0 * v.m10 + v.m01 * d1 * v.m11 + v.m02 * d2 * v.m12,
			        v.m00 * d0 * v.m20 + v.m01 * d1 * v.m21 + v.m02 * d2 * v.m22,
			        v.m10 * d0 * v.m00 + v.m11 * d1 * v.m01 + v.m12 * d2 * v.m02,
			        v.m10 * d0 * v.m10 + v.m11 * d1 * v.m11 + v.m12 * d2 * v.m12,
			        v.m10 * d0 * v.m20 + v.m11 * d1 * v.m21 + v.m12 * d2 * v.m22,
			        v.m20 * d0 * v.m00 + v.m21 * d1 * v.m01 + v.m22 * d2 * v.m02,
			        v.m20 * d0 * v.m10 + v.m21 * d1 * v.m11 + v.m22 * d2 * v.m12,
			        v.m20 * d0 * v.m20 + v.m21 * d1 * v.m21 + v.m22 * d2 * v.m22);
		}

		public static float pinv(float x, float tol){
			return (Mathf.Abs(x) < tol || Mathf.Abs(1 / x) < tol) ? 0 : (1 / x);
		}

		public static float CalcError(Mat3 A, Vector3 x, Vector3 b){
			Vector3 vtmp = new Vector3();
			vtmp = MatUtils.Vmul(A, x);
			//vtmp = b - vtmp;
			vtmp = VecUtils.Sub (b, vtmp);
			return Vector3.Dot(vtmp, vtmp);
		}

		public static float CalcError(SMat3 origA, Vector3 x, Vector3 b){
			Mat3 A = new Mat3();
			Vector3 vtmp = new Vector3();
			A.SetSymmetric(origA);
			vtmp = MatUtils.Vmul(A, x);
			vtmp = VecUtils.Sub (b, vtmp);
			//vtmp = b - vtmp;
			return Vector3.Dot(vtmp, vtmp);
		}

		public static void Rotate01(SMat3 vtav, Mat3 v){
			if (vtav.m01 == 0) {
				return;
			}
			float c = 0, s = 0;
			Schur2.Rot01(vtav, c, s);
			Givens.Rot01_post(v, c, s);
		}

		public static void Rotate02(SMat3 vtav, Mat3 v){
			if (vtav.m02 == 0) {
				return;
			}
			float c= 0, s = 0;
			Schur2.Rot02(vtav, c, s);
			Givens.Rot02_post(v, c, s);
		}

		public static void Rotate12(SMat3 vtav, Mat3 v){
			if (vtav.m12 == 0) {
				return;
			}
			float c = 0, s = 0;
			Schur2.Rot12(vtav, c, s);
			Givens.Rot12_post(v, c, s);
		}

		public static float[] CalcSymmetricGivensCoefficients(float a_pp, float a_pq, float a_qq, float c, float s){
			float [] res = new float[2];
			if (a_pq == 0) {
				res[0] = 1;
				res[1] = 0;
			}

			float tau = (a_qq - a_pp) / (2 * a_pq);
			float stt = Mathf.Sqrt(1.0f + tau * tau);
			float tan = 1.0f / ((tau >= 0) ? (tau + stt) : (tau - stt));
			res[0] = 1.0f / Mathf.Sqrt(1.0f + tan * tan);
			res[1] = tan * c;
			return res;
		}
	};

	//-------------------------------------------------------------------------------
	//Schur2
	//-------------------------------------------------------------------------------
	public static class Schur2{
		public static void Rot01(SMat3 m, float c, float s){
			float [] cs = Svd.CalcSymmetricGivensCoefficients(m.m00, m.m01, m.m11, c, s);
			float cc = cs[0] * cs[0];
			float ss = cs[1] * cs[1];
			float mix = 2 * c * s * m.m01;
			m.SetSymmetric(cc * m.m00 - mix + ss * m.m11, 0, c * m.m02 - s * m.m12,
			               ss * m.m00 + mix + cc * m.m11, s * m.m02 + c * m.m12, m.m22);
		}

		public static void Rot02(SMat3 m, float c, float s){
			float [] cs = Svd.CalcSymmetricGivensCoefficients(m.m00, m.m02, m.m22, c, s);
			float cc = cs[0] * cs[0];
			float ss = cs[1] * cs[1];
			float mix = 2 * c * s * m.m02;
			m.SetSymmetric(cc * m.m00 - mix + ss * m.m22, c * m.m01 - s * m.m12, 0,
			               m.m11, s * m.m01 + c * m.m12, ss * m.m00 + mix + cc * m.m22);

		}

		public static void Rot12(SMat3 m, float c, float s){
			float [] cs = Svd.CalcSymmetricGivensCoefficients(m.m11, m.m12, m.m22, c, s);
			float cc = cs[0] * cs[0];
			float ss = cs[1] * cs[1];
			float mix = 2 * c * s * m.m12;
			m.SetSymmetric(m.m00, c * m.m01 - s * m.m02, s * m.m01 + c * m.m02,
			               cc * m.m11 - mix + ss * m.m22, 0, ss * m.m11 + mix + cc * m.m22);
		}
	};

	//-------------------------------------------------------------------------------
	//Givens
	//-------------------------------------------------------------------------------
	public static class Givens{
		public static void Rot01_post(Mat3 m, float c, float s){
			float m00 = m.m00, m01 = m.m01, m10 = m.m10, m11 = m.m11, m20 = m.m20,
			m21 = m.m21;
			m.Set(c * m00 - s * m01, s * m00 + c * m01, m.m02, c * m10 - s * m11,
			      s * m10 + c * m11, m.m12, c * m20 - s * m21, s * m20 + c * m21, m.m22);
		}

		public static void Rot02_post(Mat3 m, float c, float s){
			float m00 = m.m00, m02 = m.m02, m10 = m.m10, m12 = m.m12,
			m20 = m.m20, m22 = m.m22 ;
			m.Set(c * m00 - s * m02, m.m01, s * m00 + c * m02, c * m10 - s * m12, m.m11,
			      s * m10 + c * m12, c * m20 - s * m22, m.m21, s * m20 + c * m22);
		}

		public static void Rot12_post(Mat3 m, float c, float s){
			float m01 = m.m01, m02 = m.m02, m11 = m.m11, m12 = m.m12,
			m21 = m.m21, m22 = m.m22;
			m.Set(m.m00, c * m01 - s * m02, s * m01 + c * m02, m.m10, c * m11 - s * m12,
			      s * m11 + c * m12, m.m20, c * m21 - s * m22, s * m21 + c * m22);
		}
	};

	//-------------------------------------------------------------------------------
	//LeastSquares
	//-------------------------------------------------------------------------------
	class LeastSquares{
		public static float solveLeastSquares(Mat3 a, Vector3 b, Vector3 x, float svd_tol, int svd_sweeps, float pinv_tol){
			Mat3 at = new Mat3();
			SMat3 ata = new SMat3();
			Vector3 atb = new Vector3();
			MatUtils.Transpose(at, a);
			MatUtils.Mmul_ata(ata, a);
			atb = MatUtils.Vmul(at, b);
			return Svd.SolveSymmetric(ata, atb, x, svd_tol, svd_sweeps, pinv_tol);
		}
	};
}
