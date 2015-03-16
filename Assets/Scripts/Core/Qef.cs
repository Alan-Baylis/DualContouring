using UnityEngine;
using System.Collections;
using QefUtils;
using System;

namespace Qef {
    public class QefData{
        public float ata_00, ata_01, ata_02, ata_11, ata_12, ata_22;
        public float atb_x, atb_y, atb_z;
        public float btb;
        public float massPoint_x, massPoint_y, massPoint_z;
        public int numPoints;
        // -------------------------------------------------------------------------------
        public QefData(){
            this.Clear();
        }
        // -------------------------------------------------------------------------------
        public QefData(float ata_00, float ata_01, float ata_02, 
		               float ata_11, float ata_12, float ata_22, 
		               float atb_x, float atb_y, float atb_z, float btb, 
		               float massPoint_x, float massPoint_y, float massPoint_z, int numPoints){
            this.Set(ata_00, ata_01, ata_02, ata_11, ata_12, ata_22, atb_x, atb_y,
                  atb_z, btb, massPoint_x, massPoint_y, massPoint_z, numPoints);
        }
        // -------------------------------------------------------------------------------
        public void Add(QefData rhs){
            this.ata_00 += rhs.ata_00;
            this.ata_01 += rhs.ata_01;
            this.ata_02 += rhs.ata_02;
            this.ata_11 += rhs.ata_11;
            this.ata_12 += rhs.ata_12;
            this.ata_22 += rhs.ata_22;
            this.atb_x += rhs.atb_x;
            this.atb_y += rhs.atb_y;
            this.atb_z += rhs.atb_z;
            this.btb += rhs.btb;
            this.massPoint_x += rhs.massPoint_x;
            this.massPoint_y += rhs.massPoint_y;
            this.massPoint_z += rhs.massPoint_z;
            this.numPoints += rhs.numPoints;
        }
        // -------------------------------------------------------------------------------
        public void Clear(){
            this.Set(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        }
        // -------------------------------------------------------------------------------
        public void Set(float ata_00, float ata_01, float ata_02, 
		                float ata_11, float ata_12, float ata_22, 
		                float atb_x, float atb_y, float atb_z, 
		                float btb, float massPoint_x, float massPoint_y, float massPoint_z, int numPoints){
            this.ata_00 = ata_00;
            this.ata_01 = ata_01;
            this.ata_02 = ata_02;
            this.ata_11 = ata_11;
            this.ata_12 = ata_12;
            this.ata_22 = ata_22;
            this.atb_x = atb_x;
            this.atb_y = atb_y;
            this.atb_z = atb_z;
            this.btb = btb;
            this.massPoint_x = massPoint_x;
            this.massPoint_y = massPoint_y;
            this.massPoint_z = massPoint_z;
            this.numPoints = numPoints;
        }

        public void Set(QefData rhs){
            this.Set(rhs.ata_00, rhs.ata_01, rhs.ata_02, rhs.ata_11, rhs.ata_12,
                  rhs.ata_22, rhs.atb_x, rhs.atb_y, rhs.atb_z, rhs.btb,
                  rhs.massPoint_x, rhs.massPoint_y, rhs.massPoint_z,
                  rhs.numPoints);
        }
        // -------------------------------------------------------------------------------
        public QefData(QefData rhs){
            this.Set(rhs);
        }
    };

    public class QefSolver{

        QefData data;
        SMat3 ata;
        Vector3 atb, massPoint, x;
        bool hasSolution;
        // -------------------------------------------------------------------------------
        public QefSolver(){
            data = new QefData();
            ata = new SMat3();
            atb = new Vector3();
            massPoint = new Vector3();
            x = new Vector3();
            hasSolution = false;
        }
        // -------------------------------------------------------------------------------
        public Vector3 GetMassPoint() { 
			return massPoint; 
		}
        // -------------------------------------------------------------------------------
        public void Add(float px, float py, float pz, float nx, float ny, float nz){
            this.hasSolution = false;
			Vector3 tmpv = new Vector3 (nx, ny, nz);
			tmpv.Normalize ();
			//VecUtils.Normalize(tmpv);
			nx = tmpv.x;
			ny = tmpv.y;
			nz = tmpv.z;
            this.data.ata_00 += nx * nx;
            this.data.ata_01 += nx * ny;
            this.data.ata_02 += nx * nz;
            this.data.ata_11 += ny * ny;
            this.data.ata_12 += ny * nz;
            this.data.ata_22 += nz * nz;
            float dot = nx * px + ny * py + nz * pz;
            this.data.atb_x += dot * nx;
            this.data.atb_y += dot * ny;
            this.data.atb_z += dot * nz;
            this.data.btb += dot * dot;
            this.data.massPoint_x += px;
            this.data.massPoint_y += py;
            this.data.massPoint_z += pz;
            this.data.numPoints++;
        }
        // -------------------------------------------------------------------------------
        public void Add(Vector3 p, Vector3 n){
            this.Add(p.x, p.y, p.z, n.x, n.y, n.z);
        }
        // -------------------------------------------------------------------------------
        public void Add(QefData rhs){
            this.hasSolution = false;
            this.data.Add(rhs);
        }
        // -------------------------------------------------------------------------------
        public QefData GetData(){
            return data;
        }
        // -------------------------------------------------------------------------------
        public float GetError(){
            if (!this.hasSolution){
                Debug.LogError("Illegal State");
            }

            return this.GetError(this.x);
        }
        // -------------------------------------------------------------------------------
        public float GetError(Vector3 pos){
            if (!this.hasSolution) {
                this.SetAta();
                this.SetAtb();
            }

            Vector3 atax = MatUtils.Vmul_symmetric(this.ata, pos);
			return VecUtils.Dot(pos, atax) - 2 * VecUtils.Dot(pos, this.atb) + this.data.btb;
        }
        // -------------------------------------------------------------------------------
        public void reset(){
            this.hasSolution = false;
            this.data.Clear();
        }
        // -------------------------------------------------------------------------------
        public float Solve(Vector3 outx, float svd_tol, int svd_sweeps, float pinv_tol){
            if (this.data.numPoints == 0) {
               // Debug.LogError("Invalid Argument");
            }

            this.massPoint = new Vector3(this.data.massPoint_x, this.data.massPoint_y, this.data.massPoint_z);
            this.massPoint = VecUtils.Scale(this.massPoint, (1.0f /(float)this.data.numPoints));

            this.SetAta();
            this.SetAtb();

            Vector3 tmpv = MatUtils.Vmul_symmetric(this.ata, this.massPoint);
            atb = VecUtils.Sub(atb, tmpv);
            this.x = new Vector3(0,0,0);

            float result = Svd.SolveSymmetric(this.ata, this.atb, this.x, svd_tol, svd_sweeps, pinv_tol);
            VecUtils.AddScaled(this.x, 1, this.massPoint);
            this.SetAtb();
            outx.Set(x.x, x.y, x.z);
            this.hasSolution = true;
            return result;
        }
        // -------------------------------------------------------------------------------
        private QefSolver(QefSolver rhs){
            data = rhs.data;
            ata = rhs.ata;
            atb = rhs.atb;
            massPoint = rhs.massPoint;
            x = rhs.x;
            hasSolution = rhs.hasSolution;
        }
        // -------------------------------------------------------------------------------
        private void SetAta(){
            this.ata.SetSymmetric(this.data.ata_00, this.data.ata_01, this.data.ata_02, 
			                      this.data.ata_11, this.data.ata_12, this.data.ata_22);
        }
        // -------------------------------------------------------------------------------
        private void SetAtb(){
            this.atb = new Vector3(this.data.atb_x, this.data.atb_y, this.data.atb_z);
        }
    };
};
