/* Objective function */
min: - 0.0054fpdi - 0.0054fpdo + 0.05apdi + 0.05apdo + 0.0002pi + 0.0002po - 0.5fsi - 0.5fso;

apdi = 0.0 hi + -0.0 li + 11.47784 ri + -0.0151 ci + -1.73455 si + -0.0 ti + 0.09664 fTempIi + 0.03001 ami + -137.97295;
asi = 0.0 hi + 0.0 li + 0.0 ri + 0.0 ci + 0.0 si + 0.0 ti + -0.0 fTempIi + 0.0003 ami + 0.01;
aTempOi = 0.0 hi + -0.0 li + 0.35024 ri + -0.1249 ci + -0.03623 si + -0.0 ti + 0.66832 fTempIi + -0.00096 ami + 6.78585;
fpdi = 0.0 hi + 0.0 li + 0.68659 ri + -1.33801 ci + -0.00116 si + -0.0 ti + -0.04419 fTempIi + 2e-05 ami + 20.73171;
fsi = 0.0 hi + 0.0 li + 0.0 ri + -0.02629 ci + 0.0 si + -0.0 ti + -0.0 fTempIi + 0.0 ami + 0.71486;
fTempOi = 0.0 hi + -0.0 li + -0.35618 ri + 0.12699 ci + 0.03633 si + -0.0 ti + 0.33467 fTempIi + -0.0006 ami + 8.16338;
pi = 0.0 hi + -0.0 li + 90.98056 ri + 3.5219 ci + -6.99429 si + 0.0 ti + 0.0 fTempIi + -0.0 ami + 244.63444;

apdo = 0.0 ho + -0.0 lo + 11.13452 ro + 0.0031 co + -1.54441 so + 0.0 to + 0.0758 fTempIo + 0.02787 amo + -118.02307;
aso = 0.0 ho + -0.0 lo + 0.01019 ro + 0.0 co + -0.00114 so + 0.0 to + -0.0001 fTempIo + 0.0003 amo + -0.04267;
aTempOo = 0.0 ho + -0.0 lo + -0.37586 ro + 0.07311 co + 0.02792 so + -0.0 to + 0.61614 fTempIo + 0.00097 amo + 4.62114;
fpdo = 0.0 ho + 0.0 lo + 0.55189 ro + -1.12279 co + 0.00473 so + -0.0 to + -0.04131 fTempIo + -2e-05 amo + 18.36662;
fso = 0.0 ho + 0.0 lo + 0.0 ro + -0.025 co + 0.0 so + -0.0 to + 0.0 fTempIo + -0.0 amo + 0.696;
fTempOo = 0.0 ho + -0.0 lo + 0.37971 ro + -0.07636 co + -0.02766 so + -0.0 to + 0.37724 fTempIo + 0.00065 amo + 10.08409;
po = 0.0 ho + -0.0 lo + 103.66548 ro + 3.52 co + -8.80379 so + 0.0 to + 0.07837 fTempIo + 8e-05 amo + 472.28901;

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
ci > 11;
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
co > 12;
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
