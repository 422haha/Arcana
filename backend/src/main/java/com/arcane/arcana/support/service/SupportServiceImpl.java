package com.arcane.arcana.support.service;

import com.arcane.arcana.common.entity.SupportRequest;
import com.arcane.arcana.support.dto.SupportDto;
import com.arcane.arcana.support.repository.SupportRepository;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.mail.javamail.JavaMailSender;
import org.springframework.mail.javamail.MimeMessageHelper;
import org.springframework.mail.javamail.MimeMessagePreparator;
import org.springframework.stereotype.Service;
import org.springframework.web.multipart.MultipartFile;
import org.thymeleaf.context.Context;
import org.thymeleaf.spring6.SpringTemplateEngine;

@Service
public class SupportServiceImpl implements SupportService {

    private final SupportRepository supportRepository;
    private final JavaMailSender mailSender;
    private final SpringTemplateEngine templateEngine;

    @Value("${spring.mail.username}")
    private String senderEmail;

    @Value("${app.domain}")
    private String appDomain;

    public SupportServiceImpl(SupportRepository supportRepository, JavaMailSender mailSender,
        SpringTemplateEngine templateEngine) {
        this.supportRepository = supportRepository;
        this.mailSender = mailSender;
        this.templateEngine = templateEngine;
    }

    @Override
    public void processSupportRequest(String category, String email, String title,
        String description, MultipartFile[] screenshots) {
        // SupportDto 생성 후 기존 메서드 호출
        SupportDto supportDto = new SupportDto();
        supportDto.setCategory(category);
        supportDto.setEmail(email);
        supportDto.setTitle(title);
        supportDto.setDescription(description);
        supportDto.setScreenshots(screenshots);

        processSupportRequest(supportDto); // 기존 메서드 호출
    }

    @Override
    public void processSupportRequest(SupportDto supportDto) {
        if (supportDto.getCategory() == null || supportDto.getCategory().isEmpty()) {
            throw new IllegalArgumentException("Category cannot be null or empty.");
        }

        // DB 저장
        SupportRequest request = new SupportRequest();
        request.setCategory(supportDto.getCategory());
        request.setEmail(supportDto.getEmail());
        request.setTitle(supportDto.getTitle());
        request.setDescription(supportDto.getDescription());
        supportRepository.save(request);

        // 이메일 전송
        sendSupportConfirmationEmail(supportDto);
    }

    private void sendSupportConfirmationEmail(SupportDto supportDto) {
        String subject = "Arcana 문의 접수 완료";

        // Thymeleaf 컨텍스트 설정
        Context context = new Context();
        context.setVariable("email", supportDto.getEmail());
        context.setVariable("title", supportDto.getTitle());

        // 템플릿을 HTML 문자열로 변환
        String htmlContent = templateEngine.process("support-confirmation", context);

        MimeMessagePreparator messagePreparator = mimeMessage -> {
            MimeMessageHelper helper = new MimeMessageHelper(mimeMessage, true, "UTF-8");
            helper.setFrom(senderEmail, "Arcana Team");
            helper.setTo(supportDto.getEmail());
            helper.setSubject(subject);
            helper.setText(htmlContent, true); // HTML 형식 설정
        };

        mailSender.send(messagePreparator);
    }
}
