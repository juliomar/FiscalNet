﻿using FiscalNet.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiscalNet.Implementacoes.Icms
{
    public class Icms10 : IIcms10
    {
        private decimal ValorProduto { get; set; }
        private decimal ValorFrete { get; set; }
        private decimal ValorSeguro { get; set; }
        private decimal DespesasAcessorias { get; set; }
        private decimal ValorIpi { get; set; }
        private decimal ValorDesconto { get; set; }
        private decimal AliquotaIcmsProprio { get; set; }
        private decimal AliquotaIcmsST { get; set; }
        private decimal Mva { get; set; }
        private decimal PercentualReducaoST { get; set; }

        private BaseIcmsProprio BCIcmsProprio { get; set; }
        private BaseIcmsST BCIcmsST { get; set; }
        private BaseReduzidaIcmsST BCReduzidaIcmsST { get; set; }

        public Icms10(decimal valorProduto,
            decimal valorFrete,
            decimal valorSeguro,
            decimal despesasAcessorias,
            decimal valorIpi,
            decimal valorDesconto,
            decimal aliqIcmsProprio,
            decimal aliqIcmsST,
            decimal mva,
            decimal percentualReducaoST = 0)
        {
            this.ValorProduto = valorProduto;
            this.ValorFrete = valorFrete;
            this.ValorSeguro = valorSeguro;
            this.DespesasAcessorias = despesasAcessorias;
            this.ValorIpi = valorIpi;
            this.ValorDesconto = valorDesconto;
            this.AliquotaIcmsProprio = aliqIcmsProprio;
            this.AliquotaIcmsST = aliqIcmsST;
            this.Mva = mva;
            this.PercentualReducaoST = percentualReducaoST;

            this.BCIcmsProprio = new BaseIcmsProprio(ValorProduto, ValorFrete, ValorSeguro, 
                DespesasAcessorias, ValorDesconto);                        
        }

        #region ICMS Próprio
        public decimal BaseIcmsProprio()
        {
            return BCIcmsProprio.CalcularBaseIcmsProprio();
        }
        public decimal ValorIcmsProprio()
        {
            return new ValorIcmsProprio(BaseIcmsProprio(), AliquotaIcmsProprio).CalcularValorIcmsProprio();
        }
        #endregion

        #region ICMSST
        public decimal BaseIcmsST()
        {
            if(PercentualReducaoST == 0)
            {
                this.BCIcmsST = new BaseIcmsST(BaseIcmsProprio(), Mva, ValorIpi);
                return BCIcmsST.CalcularBaseIcmsST();
            }
            else
            {
                this.BCReduzidaIcmsST = new BaseReduzidaIcmsST(BaseIcmsProprio(), Mva, 
                                                PercentualReducaoST, ValorIpi);
                return BCReduzidaIcmsST.CalcularBaseReduzidaIcmsST();
            }                        
        }        

        public decimal ValorIcmsST()
        {
            return new ValorIcmsST(BaseIcmsST(), AliquotaIcmsST, ValorIcmsProprio()).CalcularValorIcmsST();
        }

        public decimal ValorICMSSTDesonerado()
        {
            Icms10 icms10 = new Icms10(ValorProduto, ValorFrete, ValorSeguro,
                                     DespesasAcessorias, ValorIpi, ValorDesconto, AliquotaIcmsProprio,
                                     AliquotaIcmsST, Mva);

            decimal valorICMSSTNormal = icms10.ValorIcmsST();

            decimal valorICMSSTDesonerado = valorICMSSTNormal - ValorIcmsST();

            return decimal.Round(valorICMSSTDesonerado, 2);
        }
        #endregion
    }
}
