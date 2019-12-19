using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartDSP.Define
{
    public static class LogDefine
    {
        public enum eModule
        {
            Dist = 10,
            Recv = 20, // 사용 안할 듯(Dist 로 통일)
            Board = 30,
            Interface = 40,
            part = 50,
            Admin_User = 110,
            Admin_Dept = 100,
            Admin_Vend = 120,
            Admin_Vend_User = 130,
            Admin_Common = 140,
        }

        public enum eActionType
        {
            DistInsert = 10, // 배포 임시 저장
            DistSave = 20, // 배포 임시 저장 
            DistEoSave = 21, // 배포 EO 설정
            DistLocalFileSave = 22, // 배포 로컬파일 설정
            DistReceiverSave = 23, // 배포 로컬파일 설정
            DistReceiverDelete = 24, // 배포 로컬파일 설정
            DistStart = 30, // 배포 시작
            Expire = 40, // 만료
            Discard = 50, // 폐기
            CompulsionExpire = 60, // 강제 만료
            CompulsionDiscard = 70, // 강제 폐기
            RecvView = 80, // 배포 조회
            FileDown = 90, // 파일 다운
            FileView = 100, // PDF VIEW

            DataInsert = 110,
            DataUpdate = 120,
            DataDelete = 130,
            DataSelect = 140, //view

            FileDelete = 200 // 파일 삭제

        }

    }
}