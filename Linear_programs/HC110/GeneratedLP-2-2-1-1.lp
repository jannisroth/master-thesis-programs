/* Objective function */
min: - 0.0054fpdi - 0.0054fpdo + 0.05apdi + 0.05apdo + 0.0002pi + 0.0002po - 0.5fsi - 0.5fso;

apdi = 0.0 hi + 0.0 li + 9.67316 ri + -0.00274 ci + -1.40774 si + 0.0 ti + 0.10779 fTempIi + 0.00264 ami + -109.09996;
asi = 0.0 hi + -0.0 li + 0.0 ri + 0.0 ci + 0.0 si + 0.0 ti + 0.0 fTempIi + 3e-05 ami + 0.00571;
aTempOi = 0.0 hi + 0.0 li + 0.30168 ri + -0.02243 ci + -0.04146 si + 0.0 ti + 0.77212 fTempIi + -9e-05 ami + 6.89853;
fpdi = 0.0 hi + 0.0 li + 11.69268 ri + -6.72486 ci + -0.0204 si + 0.0 ti + -0.63581 fTempIi + 4e-05 ami + 324.07823;
fsi = 0.0 hi + -0.0 li + -2e-05 ri + -0.02468 ci + 0.0 si + 0.0 ti + 3e-05 fTempIi + -0.0 ami + 2.24736;
fTempOi = 0.0 hi + 0.0 li + -0.29836 ri + 0.02235 ci + 0.04101 si + 0.0 ti + 0.24479 fTempIi + -8e-05 ami + 9.56256;
pi = 0.0 hi + 0.0 li + 847.03542 ri + 3.32065 ci + -89.42567 si + 0.0 ti + -0.0 fTempIi + -0.0 ami + 4425.4295;

apdo = 0.0 ho + 0.0 lo + 9.78983 ro + 0.00323 co + -1.40801 so + 0.0 to + 0.10235 fTempIo + 0.00272 amo + -111.99764;
aso = 0.0 ho + -0.0 lo + 0.0 ro + 0.0 co + 0.0 so + 0.0 to + 0.0 fTempIo + 3e-05 amo + 0.00571;
aTempOo = 0.0 ho + 0.0 lo + -0.32825 ro + 0.02489 co + 0.04452 so + 0.0 to + 0.73593 fTempIo + 0.0001 amo + -0.00977;
fpdo = 0.0 ho + 0.0 lo + 11.40238 ro + -6.69773 co + 0.02184 so + 0.0 to + -0.64905 fTempIo + -5e-05 amo + 322.28288;
fso = 0.0 ho + -0.0 lo + 0.0 ro + -0.02469 co + 0.0 so + 0.0 to + -0.0 fTempIo + 0.0 amo + 2.24809;
fTempOo = 0.0 ho + 0.0 lo + 0.32384 ro + -0.02476 co + -0.04393 so + 0.0 to + 0.27263 fTempIo + 9e-05 amo + 12.33501;
po = 0.0 ho + 0.0 lo + 847.03542 ro + 3.32065 co + -89.42567 so + 0.0 to + -0.0 fTempIo + -0.0 amo + 4425.4295;

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
ri > 6;
ri < 14;
ci > 38;
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
co > 38;
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
