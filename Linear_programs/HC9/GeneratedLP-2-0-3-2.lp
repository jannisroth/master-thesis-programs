/* Objective function */
min: - 0.0054fpdi - 0.0054fpdo + 0.05apdi + 0.05apdo + 0.0002pi + 0.0002po - 0.5fsi - 0.5fso;

apdi = 0.0 hi + 0.0 li + 7.78482 ri + -0.04524 ci + -1.44525 si + 0.0 ti + 0.12388 fTempIi + 0.04013 ami + -95.67846;
asi = 0.0 hi + -0.0 li + -0.00091 ri + -0.0001 ci + -0.00039 si + -0.0 ti + 0.0003 fTempIi + 0.00043 ami + 0.02198;
aTempOi = 0.0 hi + 0.0 li + 0.2184 ri + -0.22806 ci + -0.02895 si + -0.0 ti + 0.78512 fTempIi + -0.00156 ami + 7.00751;
fpdi = 0.0 hi + 0.0 li + 2.81866 ri + -14.83438 ci + 0.00281 si + 0.0 ti + -0.19926 fTempIi + 0.00023 ami + 92.16531;
fsi = 0.0 hi + 0.0 li + -0.0 ri + -0.094 ci + 0.0 si + 0.0 ti + 0.0 fTempIi + 0.0 ami + 1.112;
fTempOi = 0.0 hi + -0.0 li + -0.21503 ri + 0.23067 ci + 0.02807 si + -0.0 ti + 0.25186 fTempIi + -0.00139 ami + 9.5356;
pi = 0.0 hi + 0.0 li + 101.29375 ri + 3.57718 ci + -8.21533 si + 0.0 ti + -0.04735 fTempIi + -0.00011 ami + 269.37887;

apdo = 0.0 ho + -0.0 lo + 9.53274 ro + 0.03175 co + -1.7377 so + -0.0 to + 0.122 fTempIo + 0.04775 amo + -134.79953;
aso = 0.0 ho + 0.0 lo + 0.0 ro + 0.0 co + 0.0 so + 0.0 to + 0.0 fTempIo + 0.00043 amo + -0.0;
aTempOo = 0.0 ho + -0.0 lo + -0.16431 ro + 0.22653 co + 0.04121 so + 0.0 to + 0.82065 fTempIo + 0.00162 amo + -2.03899;
fpdo = 0.0 ho + 0.0 lo + 26.68909 ro + -323.92116 co + 0.05499 so + -0.0 to + -1.69664 fTempIo + -0.00255 amo + 835.52964;
fso = 0.0 ho + 0.0 lo + 2e-05 ro + -0.51984 co + -1e-05 so + -0.0 to + 7e-05 fTempIo + 0.0 amo + 2.58839;
fTempOo = 0.0 ho + 0.0 lo + 0.16498 ro + -0.23095 co + -0.04157 so + -0.0 to + 0.20787 fTempIo + 0.00176 amo + 12.73933;
po = 0.0 ho + -0.0 lo + 86.75 ro + 3.66667 co + -8.49286 so + 0.0 to + 0.0 fTempIo + -0.0 amo + 225.60714;

/* Variable bounds */
aTempOi > 18.6;
1 =r6i + r8i + r10i + r12i + r14i;
ri =6r6i + 8r8i + 10r10i + 12r12i + 14r14i;
1 =s30i + s40i + s50i + s60i;
si =30s30i + 40s40i + 50s50i + 60s60i;
ti = 3;
apdi > 100;
apdi < 300;
asi > 0;
asi < 3;
aTempOi > 5;
aTempOi < 25;
fpdi > 100;
fpdi < 300;
fsi > 0;
fsi < 1.25;
fTempOi > 5;
fTempOi < 25;
pi > 0;
ri > 10;
ri < 14;
ci > 4;
ci < 60;
si > 25;
si < 60;
ti > 1;
ti < 3;
fTempIi > 5;
fTempIi < 25;

/* aTempOo < 11.4; */
1 =r6o + r8o + r10o + r12o + r14o;
ro =6r6o + 8r8o + 10r10o + 12r12o + 14r14o;
1 =s25o + s30o + s40o;
so =25s25o + 30s30o + 40s40o;
to = 1;
apdo > 100;
apdo < 300;
aso > 0;
aso < 3;
aTempOo > 5;
aTempOo < 25;
fpdo > 100;
fpdo < 300;
fso > 0;
fso < 1.25;
fTempOo > 5;
fTempOo < 25;
po > 0;
ro > 10;
ro < 14;
co > 1;
co < 4;
so > 25;
so < 60;
to > 1;
to < 3;
fTempIo > 5;
fTempIo < 25;
int hi,li,si,ti,ci,ri,ami;
int ho,lo,so,to,co,ro,amo;
bin s25i,s30i,s40i,s50i,s60i,r6i,r8i,r10i,r12i,r14i;
bin s25o,s30o,s40o,s50o,s60o,r6o,r8o,r10o,r12o,r14o;
