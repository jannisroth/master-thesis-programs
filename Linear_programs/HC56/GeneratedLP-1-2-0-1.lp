/* Objective function */
min: - 0.0054fpdi - 0.0054fpdo + 0.05apdi + 0.05apdo + 0.0002pi + 0.0002po - 0.5fsi - 0.5fso;

apdi = 0.0 hi + 0.0 li + 12.70556 ri + -0.01389 ci + -1.92159 si + 0.0 ti + 0.11655 fTempIi + 0.00618 ami + -155.19569;
asi = 0.0 hi + 0.0 li + -6e-05 ri + -0.0001 ci + -2e-05 si + -0.0 ti + -0.00011 fTempIi + 6e-05 ami + -0.01349;
aTempOi = 0.0 hi + 0.0 li + 0.28326 ri + -0.03952 ci + -0.05655 si + -0.0 ti + 0.78713 fTempIi + -0.00018 ami + 6.89517;
fpdi = 0.0 hi + 0.0 li + 26.43493 ri + -39.45845 ci + -0.06024 si + -0.0 ti + -1.3826 fTempIi + 0.00019 ami + 734.55154;
fsi = 0.0 hi + 0.0 li + -0.00012 ri + -0.08403 ci + 2e-05 si + -0.0 ti + 0.00073 fTempIi + -0.0 ami + 3.02251;
fTempOi = 0.0 hi + -0.0 li + -0.27979 ri + 0.03984 ci + 0.05593 si + -0.0 ti + 0.23268 fTempIi + -0.00016 ami + 9.85375;
pi = 0.0 hi + 0.0 li + 349.42778 ri + 3.13333 ci + -30.92952 si + 0.0 ti + 0.0 fTempIi + 0.0 ami + 1235.9127;

apdo = 0.0 ho + 0.0 lo + 10.8044 ro + 0.00452 co + -1.51027 so + -0.0 to + 0.09927 fTempIo + 0.00554 amo + -124.49863;
aso = 0.0 ho + 0.0 lo + -0.0 ro + -0.0 co + 0.0 so + -0.0 to + -0.0 fTempIo + 6e-05 amo + -0.02;
aTempOo = 0.0 ho + 0.0 lo + -0.33388 ro + 0.04677 co + 0.04423 so + 0.0 to + 0.72622 fTempIo + 0.00018 amo + 0.27914;
fpdo = 0.0 ho + 0.0 lo + 9.6409 ro + -10.16211 co + 0.01855 so + -0.0 to + -0.55851 fTempIo + -8e-05 amo + 275.76167;
fso = 0.0 ho + 0.0 lo + 0.0 ro + -0.041 co + -0.0 so + -0.0 to + 0.0 fTempIo + 0.0 amo + 2.11986;
fTempOo = 0.0 ho + 0.0 lo + 0.3281 ro + -0.04521 co + -0.04353 so + -0.0 to + 0.2826 fTempIo + 0.00017 amo + 12.14964;
po = 0.0 ho + -0.0 lo + 442.85 ro + 3.14 co + -45.6852 so + -0.0 to + -0.0 fTempIo + -0.0 amo + 2312.304;

/* Variable bounds */
aTempOi > 18.6;
1 =r6i + r8i + r10i + r12i + r14i;
ri =6r6i + 8r8i + 10r10i + 12r12i + 14r14i;
1 =s25i + s30i + s40i;
si =25s25i + 30s30i + 40s40i;
ti = 2;
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
ci > 1;
ci < 22;
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
co > 22;
co < 60;
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
