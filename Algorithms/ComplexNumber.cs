using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSPAlgorithms.DataStructures;

namespace DSPAlgorithms.Algorithms
{
    public class ComplexNumber
    {
       public double Reyal { get; set; }
       public double Imagenary { get; set; }

        public double ComplexToDouble()
        {
            double result = this.Reyal + this.Imagenary;
            return result;
        }
        public ComplexNumber()
        {

        }
        public ComplexNumber(double R , double i)
        {
            Reyal = R;
            Imagenary = i;
        }
        public ComplexNumber Add(ComplexNumber Num1, ComplexNumber Num2)
        {
            ComplexNumber sum=new ComplexNumber(Num1.Reyal+Num2.Reyal ,Num1.Imagenary+Num2.Imagenary);
            return sum;
        }
        public void AddNumber(double num)
        {
            this.Reyal += num;
            this.Imagenary += num;
        }

        public void SubtractionNumber(double num)
        {
            this.Reyal -= num;
            this.Imagenary -= num;

        }

        public ComplexNumber Subtraction(ComplexNumber Num1, ComplexNumber Num2)
        {
            ComplexNumber Sub = new ComplexNumber(Num1.Reyal - Num2.Reyal, Num1.Imagenary - Num2.Imagenary);
            return Sub;
        }
        public  ComplexNumber Multiblication(ComplexNumber Num1, ComplexNumber Num2)
        {
            ComplexNumber Mul = new ComplexNumber(0, 0);
            Mul.Reyal = Num1.Reyal * Num2.Reyal - (Num1.Imagenary * Num2.Imagenary);
            Mul.Imagenary = Num1.Reyal * Num2.Imagenary + Num1.Imagenary * Num2.Reyal;
            
            return Mul;
        }

        public void Equal(ComplexNumber Num)
        {
            this.Reyal = Num.Reyal;
            this.Imagenary = Num.Imagenary;
            
        }
        public  void MultiblyByANumber(double Num)
        {
            this.Reyal = Num * this.Reyal;
            this.Imagenary = Num * this.Imagenary;
        }

        public void DividedByANumber(double Num)
        {
            this.Reyal =  this.Reyal / Num;
            this.Imagenary = this.Imagenary / Num;
        }

        public  void EqualZero()
        {
            this.Reyal = 0;
            this.Imagenary = 0;
        }
       

    }
}
