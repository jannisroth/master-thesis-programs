/* Objective function */
min: - 0.0054fpdi - 0.0054fpdo + 0.05apdi + 0.05apdo + 0.0002pi + 0.0002po - 0.5fsi - 0.5fso;

apdi = 0.0 hi + 0.0 li + 12.32487 ri + -0.02268 ci + -1.8594 si + -0.0 ti + 0.12236 fTempIi + 0.02312 ami + -149.50175;
asi = 0.0 hi + -0.0 li + 0.0 ri + 0.0 ci + 0.0 si + 0.0 ti + -0.0 fTempIi + 0.00023 ami + -0.01;
aTempOi = 0.0 hi + 0.0 li + 0.28628 ri + -0.14819 ci + -0.04851 si + -0.0 ti + 0.77672 fTempIi + -0.00071 ami + 6.98956;
fpdi = 0.0 hi + 0.0 li + 10.30097 ri + -36.86659 ci + -0.02183 si + -0.0 ti + -0.56221 fTempIi + 0.00029 ami + 288.8625;
fsi = 0.0 hi + 0.0 li + -1e-05 ri + -0.13485 ci + 0.0 si + -0.0 ti + 4e-05 fTempIi + -0.0 ami + 1.98429;
fTempOi = 0.0 hi + 0.0 li + -0.28485 ri + 0.14893 ci + 0.04795 si + -0.0 ti + 0.24068 fTempIi + -0.0006 ami + 9.61678;
pi = 0.0 hi + 0.0 li + 123.90556 ri + 4.14667 ci + -11.27524 si + 0.0 ti + 0.0 fTempIi + 0.0 ami + 405.94921;

apdo = 0.0 ho + 0.0 lo + 10.46964 ro + 0.01667 co + -1.46471 so + 0.0 to + 0.09755 fTempIo + 0.02065 amo + -118.51287;
aso = 0.0 ho + 0.0 lo + -0.0 ro + -0.0 co + 0.0 so + 0.0 to + -0.0 fTempIo + 0.00023 amo + -0.01;
aTempOo = 0.0 ho + -0.0 lo + -0.28961 ro + 0.12388 co + 0.04599 so + 0.0 to + 0.76886 fTempIo + 0.00071 amo + -0.34847;
fpdo = 0.0 ho + -0.0 lo + 67.3035 ro + -509.43452 co + 0.12194 so + 0.0 to + -3.45682 fTempIo + -0.00207 amo + 1835.45042;
fso = 0.0 ho + -0.0 lo + 0.00021 ro + -0.5744 co + -3e-05 so + 0.0 to + 0.00104 fTempIo + 0.0 amo + 4.00945;
fTempOo = 0.0 ho + 0.0 lo + 0.28472 ro + -0.12012 co + -0.04548 so + 0.0 to + 0.23889 fTempIo + 0.00071 amo + 12.29751;
po = 0.0 ho + 0.0 lo + 142.93125 ro + 3.6 co + -13.118 so + -0.0 to + 0.0 fTempIo + 0.0 amo + 620.5475;

/* Variable bounds */
aTempOi > 18.6;
1 =r6i + r8i + r10i + r12i + r14i;
ri =6r6i + 8r8i + 10r10i + 12r12i + 14r14i;
1 =s25i + s30i + s40i;
si =25s25i + 30s30i + 40s40i;
ti = 1;
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
ri > 6;
ri < 14;
ci > 5;
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
1 =s30o + s40o + s50o + s60o;
so =30s30o + 40s40o + 50s50o + 60s60o;
to = 3;
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
ro > 6;
ro < 14;
co > 1;
co < 5;
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
