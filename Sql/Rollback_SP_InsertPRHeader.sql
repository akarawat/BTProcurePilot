-- Rolls SP_InsertPRHeader back to its state before
-- Sql/Alter_SP_InsertPRHeader_AddRevisionTracking.sql (V302).
-- No longer needed: the actual resubmit flow lives entirely in the
-- separate SP_ResubmitPRHeader (see Sql/Create_SP_ResubmitPRHeader.sql),
-- so this revert removes now-unreachable/dead logic from SP_InsertPRHeader.
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- V300
-- V301 Revise Docno Format {dep}-{YYMM}{3Digits per month}
-- =============================================
ALTER PROCEDURE [dbo].[SP_InsertPRHeader]
	@prno VARCHAR(50)  = NULL,
    @projectno VARCHAR(50)  = NULL,
    @empcode VARCHAR(25)  = NULL,
    @approx_type INT  = NULL,
    @approx_dt DATETIME  = NULL,
    @invcreditno VARCHAR(50)  = NULL,
    @purpose_type INT  = NULL,
    @ref_docs VARCHAR(50)  = NULL,
    @pr_reason NVARCHAR(255)  = NULL,
    @pr_recvdt DATETIME  = NULL,
    @pr_recvpono VARCHAR(50)  = NULL,
    @attach_flag VARCHAR(20)  = NULL,
    @reqDepCode VARCHAR(15)  = NULL,
    @reqDate DATE = NULL,
    @reqFlag INT  = NULL,
    @appEmp VARCHAR(15)  = NULL,
	@appEmp2 VARCHAR(15)  = NULL,
    --@appDate DATETIME  = NULL,
    --@appFlag INT  = NULL,
    @countEmp VARCHAR(15)  = NULL,
    --@countDate DATETIME  = NULL,
    --@countFlag INT  = NULL,
    @authEmp VARCHAR(15)  = NULL,
    --@authDate DATETIME  = NULL,
    --@authFlag INT  = NULL,
    @prstatus INT,
	@pub_remark text = NULL,
	@prcurrency nvarchar(25) = NULL,
    --@create_dt DATETIME,
    --@update_dt DATETIME,

	@id_supp1 varchar(15) = NULL,
	@name_supp1 nvarchar(255) = NULL,
	@vc_supp1 varchar(25) = NULL,
	@contact_supp1 varchar(150) = NULL,
	@email_supp1 varchar(255) = NULL,
	@tel_supp1 varchar(150) = NULL,
	@remark_supp1 nvarchar(255) = NULL,
	@quoref_supp1 varchar(50) = NULL,
	@refnodt_supp1 DATETIME = NULL,
	--@currency1 nvarchar(25) = NULL,

	@id_supp2  varchar(15)  =  NULL,
	@name_supp2  nvarchar(255)  =  NULL,
	@vc_supp2  varchar(25)  =  NULL,
	@contact_supp2  varchar(150)  =  NULL,
	@email_supp2  varchar(255)  =  NULL,
	@tel_supp2  varchar(150)  =  NULL,
	@remark_supp2  nvarchar(255)  =  NULL,
	@quoref_supp2  varchar(50)  =  NULL,
	@refnodt_supp2  DATETIME  =  NULL,
	--@currency2 nvarchar(25) = NULL,

	@id_supp3  varchar(15)  =  NULL,
	@name_supp3  nvarchar(255)  =  NULL,
	@vc_supp3  varchar(25)  =  NULL,
	@contact_supp3  varchar(150)  =  NULL,
	@email_supp3  varchar(255)  =  NULL,
	@tel_supp3  varchar(150)  =  NULL,
	@remark_supp3  nvarchar(255)  =  NULL,
	@quoref_supp3  varchar(50)  =  NULL,
	@refnodt_supp3  DATETIME  =  NULL,
	--@currency3 nvarchar(25) = NULL,

	@id_supp4  varchar(15)  =  NULL,
	@name_supp4  nvarchar(255)  =  NULL,
	@vc_supp4  varchar(25)  =  NULL,
	@contact_supp4  varchar(150)  =  NULL,
	@email_supp4  varchar(255)  =  NULL,
	@tel_supp4  varchar(150)  =  NULL,
	@remark_supp4  nvarchar(255)  =  NULL,
	@quoref_supp4  varchar(50)  =  NULL,
	@refnodt_supp4  DATETIME  =  NULL,

	@id_supp5  varchar(15)  =  NULL,
	@name_supp5  nvarchar(255)  =  NULL,
	@vc_supp5  varchar(25)  =  NULL,
	@contact_supp5  varchar(150)  =  NULL,
	@email_supp5  varchar(255)  =  NULL,
	@tel_supp5  varchar(150)  =  NULL,
	@remark_supp5  nvarchar(255)  =  NULL,
	@quoref_supp5  varchar(50)  =  NULL,
	@refnodt_supp5  DATETIME  =  NULL,

	@RTN_PROJNO VARCHAR(50) OUTPUT,
    @RESULT INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
	DECLARE @TMPID UNIQUEIDENTIFIER;
	SET @TMPID = NEWID();
	-- ตรวจสอบก่อนว่า เป็น Draft หรือ PR


	-- A: ถ้าเป็น Draft
	IF @prstatus = 0
	 BEGIN
		-- ถ้าเป็น DRAFT ใหม่
		IF EXISTS (SELECT 1 FROM PRHeader WHERE prno = @prno)
		 BEGIN
			SET @RESULT = 1 -- พบ Draft prno = Update
			SET @RTN_PROJNO = @prno;
			DECLARE @PRE_UDPPRID UNIQUEIDENTIFIER;
			SELECT @PRE_UDPPRID = id FROM PRHeader WHERE prno = @prno;
			--UPDATE PRHeader SET projectno=@projectno,empcode=@empcode,approx_type=@approx_type,approx_dt=@approx_dt,invcreditno=@invcreditno,
			-- purpose_type=@purpose_type,ref_docs=@ref_docs,pr_reason=@pr_reason,pr_recvdt=@pr_recvdt,pr_recvpono=@pr_recvpono,attach_flag = @attach_flag,
			-- reqDepCode = @reqDepCode,reqFlag = @reqFlag,appEmp = @appEmp,appDate = @appDate,appFlag = @appFlag,countEmp = @countEmp,
			-- countDate = @countDate,countFlag = @countFlag,authEmp = @authEmp,authDate = @authDate,authFlag = @authFlag, prstatus=0,
			-- pub_remark = @pub_remark, update_dt = getdate(), prcurrency = @prcurrency
			-- WHERE id = @PRE_UDPPRID;

			UPDATE PRHeader SET projectno=@projectno,empcode=@empcode,approx_type=@approx_type,approx_dt=@approx_dt,invcreditno=@invcreditno,
			 purpose_type=@purpose_type,ref_docs=@ref_docs,pr_reason=@pr_reason,pr_recvdt=@pr_recvdt,pr_recvpono=@pr_recvpono,attach_flag = @attach_flag,
			 reqDepCode = @reqDepCode, reqFlag = @reqFlag, appEmp = @appEmp, appEmp2 = @appEmp2, countEmp = @countEmp,
			 authEmp = @authEmp, prstatus=0,
			 pub_remark = @pub_remark, update_dt = getdate(), prcurrency = @prcurrency
			 WHERE id = @PRE_UDPPRID;

			--> Upd Supplier 1
			DECLARE @Pre_SuppID1 UNIQUEIDENTIFIER;
			SELECT @Pre_SuppID1 = id FROM PRSuggVendor WHERE ref_prid = @PRE_UDPPRID AND sugg_item = 1;
			IF @Pre_SuppID1 IS NULL
			 BEGIN
				INSERT INTO [dbo].[PRSuggVendor] (
				 [id], [ref_prid], [sugg_item], [vencode], [venvc], [vencontact], [venemail], [ventelfax],
				 [venremark], [name_supp], quoref_supp, refnodt_supp, [create_dt], [update_dt]
				) VALUES (
				 NEWID(), @PRE_UDPPRID, 1, @id_supp1, @vc_supp1, @contact_supp1, @email_supp1, @tel_supp1,
				 @remark_supp1, @name_supp1, @quoref_supp1, @refnodt_supp1, GETDATE(), GETDATE()
				);
			 END
			ELSE
			 BEGIN
				UPDATE [dbo].[PRSuggVendor] SET
				 [vencode] = @id_supp1, [venvc] = @vc_supp1, [vencontact] = @contact_supp1, [venemail] = @email_supp1, [ventelfax] = @tel_supp1,
				 [venremark] = @remark_supp1, [name_supp] = @name_supp1, quoref_supp = @quoref_supp1, refnodt_supp = @refnodt_supp1, [update_dt] = GETDATE()

				WHERE id = @Pre_SuppID1;
			 END
			--> Upd Supplier 2
			DECLARE @Pre_SuppID2 UNIQUEIDENTIFIER;
			SELECT @Pre_SuppID2 = id FROM PRSuggVendor WHERE ref_prid = @PRE_UDPPRID AND sugg_item = 2;
			IF @Pre_SuppID2 IS NULL
			 BEGIN
				INSERT INTO [dbo].[PRSuggVendor] (
				 [id], [ref_prid], [sugg_item], [vencode], [venvc], [vencontact], [venemail], [ventelfax],
				 [venremark], [name_supp], quoref_supp, refnodt_supp, [create_dt], [update_dt]
				) VALUES (
				 NEWID(), @PRE_UDPPRID, 2, @id_supp2, @vc_supp2, @contact_supp2, @email_supp2, @tel_supp2,
				 @remark_supp2, @name_supp2, @quoref_supp2, @refnodt_supp2, GETDATE(), GETDATE()
				);
			 END
			ELSE
			 BEGIN
				UPDATE [dbo].[PRSuggVendor] SET
				 [vencode] = @id_supp2, [venvc] = @vc_supp2, [vencontact] = @contact_supp2, [venemail] = @email_supp2, [ventelfax] = @tel_supp2,
				 [venremark] = @remark_supp2, [name_supp] = @name_supp2, quoref_supp = @quoref_supp2, refnodt_supp = @refnodt_supp2, [update_dt] = GETDATE()

				WHERE id = @Pre_SuppID2;
			 END
			--> Upd Supplier 3
			DECLARE @Pre_SuppID3 UNIQUEIDENTIFIER;
			SELECT @Pre_SuppID3 = id FROM PRSuggVendor WHERE ref_prid = @PRE_UDPPRID AND sugg_item = 3;
			IF @Pre_SuppID3 IS NULL
			 BEGIN
				INSERT INTO [dbo].[PRSuggVendor] (
				 [id], [ref_prid], [sugg_item], [vencode], [venvc], [vencontact], [venemail], [ventelfax],
				 [venremark], [name_supp], quoref_supp, refnodt_supp, [create_dt], [update_dt]
				) VALUES (
				 NEWID(), @PRE_UDPPRID, 3, @id_supp3, @vc_supp3, @contact_supp3, @email_supp3, @tel_supp3,
				 @remark_supp3, @name_supp3, @quoref_supp3, @refnodt_supp3, GETDATE(), GETDATE()
				);
			 END
			ELSE
			 BEGIN
				UPDATE [dbo].[PRSuggVendor] SET
				 [vencode] = @id_supp3, [venvc] = @vc_supp3, [vencontact] = @contact_supp3, [venemail] = @email_supp3, [ventelfax] = @tel_supp3,
				 [venremark] = @remark_supp3, [name_supp] = @name_supp3, quoref_supp = @quoref_supp3, refnodt_supp = @refnodt_supp3, [update_dt] = GETDATE()

				WHERE id = @Pre_SuppID3;
			 END
			--> Upd Supplier 4
			DECLARE @Pre_SuppID4 UNIQUEIDENTIFIER;
			SELECT @Pre_SuppID4 = id FROM PRSuggVendor WHERE ref_prid = @PRE_UDPPRID AND sugg_item = 4;
			IF @Pre_SuppID4 IS NULL
			 BEGIN
				INSERT INTO [dbo].[PRSuggVendor] (
				 [id], [ref_prid], [sugg_item], [vencode], [venvc], [vencontact], [venemail], [ventelfax],
				 [venremark], [name_supp], quoref_supp, refnodt_supp, [create_dt], [update_dt]
				) VALUES (
				 NEWID(), @PRE_UDPPRID, 4, @id_supp4, @vc_supp4, @contact_supp4, @email_supp4, @tel_supp4,
				 @remark_supp4, @name_supp4, @quoref_supp4, @refnodt_supp4, GETDATE(), GETDATE()
				);
			 END
			ELSE
			 BEGIN
				UPDATE [dbo].[PRSuggVendor] SET
				 [vencode] = @id_supp4, [venvc] = @vc_supp4, [vencontact] = @contact_supp4, [venemail] = @email_supp4, [ventelfax] = @tel_supp4,
				 [venremark] = @remark_supp4, [name_supp] = @name_supp4, quoref_supp = @quoref_supp4, refnodt_supp = @refnodt_supp4, [update_dt] = GETDATE()

				WHERE id = @Pre_SuppID4;
			 END
			--> Upd Supplier 5
			DECLARE @Pre_SuppID5 UNIQUEIDENTIFIER;
			SELECT @Pre_SuppID5 = id FROM PRSuggVendor WHERE ref_prid = @PRE_UDPPRID AND sugg_item = 5;
			IF @Pre_SuppID5 IS NULL
			 BEGIN
				INSERT INTO [dbo].[PRSuggVendor] (
				 [id], [ref_prid], [sugg_item], [vencode], [venvc], [vencontact], [venemail], [ventelfax],
				 [venremark], [name_supp], quoref_supp, refnodt_supp, [create_dt], [update_dt]
				) VALUES (
				 NEWID(), @PRE_UDPPRID, 5, @id_supp5, @vc_supp5, @contact_supp5, @email_supp5, @tel_supp5,
				 @remark_supp5, @name_supp5, @quoref_supp5, @refnodt_supp5, GETDATE(), GETDATE()
				);
			 END
			ELSE
			 BEGIN
				UPDATE [dbo].[PRSuggVendor] SET
				 [vencode] = @id_supp5, [venvc] = @vc_supp5, [vencontact] = @contact_supp5, [venemail] = @email_supp5, [ventelfax] = @tel_supp5,
				 [venremark] = @remark_supp5, [name_supp] = @name_supp5, quoref_supp = @quoref_supp5, refnodt_supp = @refnodt_supp5, [update_dt] = GETDATE()

				WHERE id = @Pre_SuppID5;
			 END

			RETURN;
		 END
		ELSE
		 BEGIN
			SET @RESULT = 1 -- ไม่พบ Draft prno = Insert
			SET @RTN_PROJNO = @prno;
			INSERT INTO PRHeader (
			 id,prno,projectno,empcode,approx_type,approx_dt,invcreditno,purpose_type,ref_docs,pr_reason,pr_recvdt,pr_recvpono,
			 attach_flag,reqDepCode,reqDate,reqFlag,appEmp,appEmp2,countEmp,authEmp,
			 prstatus,pub_remark, create_dt, update_dt, prcurrency, total_disc, quo_return
			)
			VALUES (
			 @TMPID,@prno,@projectno,@empcode,@approx_type,@approx_dt,@invcreditno,@purpose_type,@ref_docs,@pr_reason,@pr_recvdt,
			 @pr_recvpono,@attach_flag,@reqDepCode,GETDATE(),@reqFlag,@appEmp,@appEmp2,@countEmp,
			 @authEmp,@prstatus,@pub_remark,GETDATE(),GETDATE(),@prcurrency, 0, 0
			)

			--> Insert Supplier 1
			INSERT INTO [dbo].[PRSuggVendor] (
			 [id], [ref_prid], [sugg_item], [vencode], [venvc], [vencontact], [venemail], [ventelfax],
			 [venremark], [name_supp], quoref_supp, refnodt_supp, [create_dt], [update_dt]
			) VALUES (
			 NEWID(), @TMPID, 1, @id_supp1, @vc_supp1, @contact_supp1, @email_supp1, @tel_supp1,
			 @remark_supp1, @name_supp1, @quoref_supp1, @refnodt_supp1, GETDATE(), GETDATE()
			);
			--> Insert Supplier 2
			INSERT INTO [dbo].[PRSuggVendor] (
			 [id], [ref_prid], [sugg_item], [vencode], [venvc], [vencontact], [venemail], [ventelfax],
			 [venremark], [name_supp], quoref_supp, refnodt_supp, [create_dt], [update_dt]
			) VALUES (
			 NEWID(), @TMPID, 2, @id_supp2, @vc_supp2, @contact_supp2, @email_supp2, @tel_supp2,
			 @remark_supp2, @name_supp2, @quoref_supp2, @refnodt_supp2, GETDATE(), GETDATE()
			);

			--> Insert Supplier 3
			INSERT INTO [dbo].[PRSuggVendor] (
			 [id], [ref_prid], [sugg_item], [vencode], [venvc], [vencontact], [venemail], [ventelfax],
			 [venremark], [name_supp], quoref_supp, refnodt_supp, [create_dt], [update_dt]
			) VALUES (
			 NEWID(), @TMPID, 3, @id_supp3, @vc_supp3, @contact_supp3, @email_supp3, @tel_supp3,
			 @remark_supp3, @name_supp3, @quoref_supp3, @refnodt_supp3, GETDATE(), GETDATE()
			);
		 RETURN;
		 END
	 END
	-- B: ถ้าเป็น PR
	IF @prstatus = 1
	 BEGIN

		--->DECLARE @prno VARCHAR(50) = 'S03434-250806001';
		--->DECLARE @reqDate DATETIME = '2025-08-06';
		--->DECLARE @reqDepCode VARCHAR(15) = '420';
		--->DECLARE @projectno VARCHAR(50) = '911';

		--select * from PRHeader

		DECLARE @UID_PPRID UNIQUEIDENTIFIER;
		DECLARE @CREATEDATE DATETIME;
		SELECT @UID_PPRID = id, @CREATEDATE = create_dt FROM PRHeader WHERE prno = @prno;

		-- Count Department PR by daily
		--->DECLARE @projectno VARCHAR(50) = '91';
		--->DECLARE @reqDepCode  VARCHAR(10) = '420';
		--->DECLARE @reqDate     DATE        = GETDATE(); -- หรือวันอื่นที่กำหนด

		--SET @reqDate = GETDATE();
		SET @reqDate = @CREATEDATE;
		DECLARE @prefix      VARCHAR(25);
		DECLARE @running     INT;
		DECLARE @doc_number  VARCHAR(50);

		-- สร้าง prefix เช่น P420-250806
		--DECLARE @PR_PROJNO VARCHAR(50) = '';
		--IF @projectno IS NOT NULL AND LEN(@projectno) > 0
		-- BEGIN
		--  SET @reqDepCode = CONCAT('P', @reqDepCode);
		-- END

		--V300 ตรวจสอบเลข running ล่าสุดเฉพาะ prefix นี้ (วันเดียวกัน)
		/*
		SET @prefix = CONCAT(@reqDepCode, '-', CONVERT(CHAR(6), @reqDate, 12));
		SELECT @running = ISNULL(
		 (SELECT MAX(CAST(RIGHT(prno, 3) AS INT))
		  FROM dbo.PRHeader WITH (UPDLOCK, HOLDLOCK)
		  WHERE prno LIKE @prefix + '%'
		  --and CONVERT(DATE, reqDate) = convert(DATE, getdate())
		  and CONVERT(DATE, reqDate) = convert(DATE, @CREATEDATE)
		  ),
		 0
		) + 1;
		*/
		--V301 รูปแบบใหม่ ไม่แยก dep นับรวมกันหมดในเดือนนั้นๆ
		---> Dev
		/*
		PRINT CONVERT(CHAR(5), '2026-01-10 09:45:52', 12);
		select top(20) * FROM dbo.PRHeader order by create_dt desc;
		DECLARE @prno VARCHAR(25) = 'S00518-260110440';
		DECLARE @prefix      VARCHAR(20);
		DECLARE @running     INT;
		DECLARE @doc_number  VARCHAR(50);
		DECLARE @CREATEDATE DATETIME;
		SELECT @CREATEDATE = create_dt FROM PRHeader WHERE prno = @prno;
		*/
		/* เช็คห้ามซ้ำ เก่า
		--SET @prefix = CONCAT('-', CONVERT(CHAR(4), @CREATEDATE, 12));
		SET @prefix = CONCAT('-', CONVERT(CHAR(4), GETDATE(), 12));
		--PRINT CONVERT(CHAR(4), GETDATE(), 12);
		--PRINT @prefix;
		SELECT @running = ISNULL(
		 (SELECT COUNT(*)
		  FROM dbo.PRHeader
		  --WHERE create_dt >= DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)
		  --  AND create_dt < DATEADD(MONTH, 1, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1))
		  WHERE YEAR(create_dt) = YEAR(GETDATE())
		  AND MONTH(create_dt) = MONTH(GETDATE())
		  ),
		 1
		) + 1;

		-- สร้างเลขเอกสาร เช่น P420-250806001 แต่ถ้าเป็น PR อยู่แล้วไม่ต้องสร้าง
		SET @doc_number = CONCAT(@reqDepCode, @prefix, RIGHT('000' + CAST(@running AS VARCHAR(3)), 3));
		--- Check อีกรอบว่าซ้ำไหม
		IF EXISTS (SELECT 1 FROM PRHeader WHERE prno = @doc_number)
		 BEGIN
		  SET @doc_number = CONCAT(@reqDepCode, @prefix, RIGHT('000' + CAST((@running + 1) AS VARCHAR(3)), 3));
		 END
		*/
		-- เช็คห้ามซ้ำใหม่
		SET @prefix = CONCAT('-', CONVERT(CHAR(4), GETDATE(), 12));

		-- นับจำนวน PR ของเดือนปัจจุบัน
		SELECT @running = COUNT(*)
		FROM dbo.PRHeader
		WHERE YEAR(create_dt) = YEAR(GETDATE())
		  AND MONTH(create_dt) = MONTH(GETDATE());
		SET @running = ISNULL(@running, 0) + 1;
		-- สร้างเลขเริ่มต้น
		SET @doc_number = CONCAT(
		 --@reqDepCode,
		 @prefix,
		 RIGHT('000' + CAST(@running AS VARCHAR(3)), 3)
		);
		--PRINT @doc_number;
		-- 🔁 วนจนกว่าจะไม่เจอ PR ซ้ำ
		WHILE EXISTS (SELECT 1 FROM dbo.PRHeader WHERE prno LIKE '%' + @doc_number)
		BEGIN
		 SET @running = @running + 1;

		 SET @doc_number = CONCAT(
		  --@reqDepCode,
		  @prefix,
		  RIGHT('000' + CAST(@running AS VARCHAR(3)), 3)
		 );
		END

		-- รวม Docno
		SET @doc_number = CONCAT(
		  @reqDepCode,
		  @doc_number);

		--IF @prno LIKE '%S%'
		-- BEGIN
		--  SET @doc_number = CONCAT(@reqDepCode, @prefix, RIGHT('000' + CAST(@running AS VARCHAR(3)), 3));
		-- END
		--ELSE
		-- BEGIN
		--  SET @doc_number = @prno;
		-- END
		--->PRINT @doc_number;


		-- UPDATE มาจาก DRAFT PR
		SET @RESULT = 1 -- พบ Draft prno = Update
		SET @RTN_PROJNO = @doc_number;

		UPDATE PRHeader SET prno=@doc_number, projectno=@projectno,empcode=@empcode,approx_type=@approx_type,approx_dt=@approx_dt,invcreditno=@invcreditno,
		 purpose_type=@purpose_type,ref_docs=@ref_docs,pr_reason=@pr_reason,pr_recvdt=@pr_recvdt,pr_recvpono=@pr_recvpono,attach_flag = @attach_flag,
		 reqDepCode = @reqDepCode,reqFlag = @reqFlag,appEmp = @appEmp,appEmp2 = @appEmp2,countEmp = @countEmp,
		 authEmp = @authEmp, prstatus=1,
		 pub_remark = @pub_remark, update_dt = getdate(), prcurrency = @prcurrency
		 WHERE id = @UID_PPRID;

		--> Upd Supplier 1
		DECLARE @SuppID1 UNIQUEIDENTIFIER;
		SELECT @SuppID1 = id FROM PRSuggVendor WHERE ref_prid = @UID_PPRID AND sugg_item = 1;
		IF @SuppID1 IS NULL
		 BEGIN
			INSERT INTO [dbo].[PRSuggVendor] (
			 [id], [ref_prid], [sugg_item], [vencode], [venvc], [vencontact], [venemail], [ventelfax],
			 [venremark], [name_supp], quoref_supp, refnodt_supp, [create_dt], [update_dt]
			) VALUES (
			 NEWID(), @UID_PPRID, 1, @id_supp1, @vc_supp1, @contact_supp1, @email_supp1, @tel_supp1,
			 @remark_supp1, @name_supp1, @quoref_supp1, @refnodt_supp1, GETDATE(), GETDATE()
			);
		 END
		ELSE
		 BEGIN
			UPDATE [dbo].[PRSuggVendor] SET
			 [vencode] = @id_supp1, [venvc] = @vc_supp1, [vencontact] = @contact_supp1, [venemail] = @email_supp1, [ventelfax] = @tel_supp1,
			 [venremark] = @remark_supp1, [name_supp] = @name_supp1, quoref_supp = @quoref_supp1, refnodt_supp = @refnodt_supp1, [update_dt] = GETDATE()

			WHERE id = @SuppID1;
		 END
		--> Upd Supplier 2
		DECLARE @SuppID2 UNIQUEIDENTIFIER;
		SELECT @SuppID2 = id FROM PRSuggVendor WHERE ref_prid = @UID_PPRID AND sugg_item = 2;
		IF @SuppID2 IS NULL
		 BEGIN
			INSERT INTO [dbo].[PRSuggVendor] (
			 [id], [ref_prid], [sugg_item], [vencode], [venvc], [vencontact], [venemail], [ventelfax],
			 [venremark], [name_supp], quoref_supp, refnodt_supp, [create_dt], [update_dt]
			) VALUES (
			 NEWID(), @UID_PPRID, 2, @id_supp2, @vc_supp2, @contact_supp2, @email_supp2, @tel_supp2,
			 @remark_supp2, @name_supp2, @quoref_supp2, @refnodt_supp2, GETDATE(), GETDATE()
			);
		 END
		ELSE
		 BEGIN
			UPDATE [dbo].[PRSuggVendor] SET
			 [vencode] = @id_supp2, [venvc] = @vc_supp2, [vencontact] = @contact_supp2, [venemail] = @email_supp2, [ventelfax] = @tel_supp2,
			 [venremark] = @remark_supp2, [name_supp] = @name_supp2, quoref_supp = @quoref_supp2, refnodt_supp = @refnodt_supp2, [update_dt] = GETDATE()

			WHERE id = @SuppID2;
		 END
		--> Upd Supplier 3
		DECLARE @SuppID3 UNIQUEIDENTIFIER;
		SELECT @SuppID3 = id FROM PRSuggVendor WHERE ref_prid = @UID_PPRID AND sugg_item = 3;
		IF @SuppID3 IS NULL
		 BEGIN
			INSERT INTO [dbo].[PRSuggVendor] (
			 [id], [ref_prid], [sugg_item], [vencode], [venvc], [vencontact], [venemail], [ventelfax],
			 [venremark], [name_supp], quoref_supp, refnodt_supp, [create_dt], [update_dt]
			) VALUES (
			 NEWID(), @UID_PPRID, 3, @id_supp3, @vc_supp3, @contact_supp3, @email_supp3, @tel_supp3,
			 @remark_supp3, @name_supp3, @quoref_supp3, @refnodt_supp3, GETDATE(), GETDATE()
			);
		 END
		ELSE
		 BEGIN
			UPDATE [dbo].[PRSuggVendor] SET
			 [vencode] = @id_supp3, [venvc] = @vc_supp3, [vencontact] = @contact_supp3, [venemail] = @email_supp3, [ventelfax] = @tel_supp3,
			 [venremark] = @remark_supp3, [name_supp] = @name_supp3, quoref_supp = @quoref_supp3, refnodt_supp = @refnodt_supp3, [update_dt] = GETDATE()

			WHERE id = @SuppID3;
		 END

		SET @RESULT = 1 -- Insert สำเร็จ
	 END


END
