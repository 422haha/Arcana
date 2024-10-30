package com.arcane.arcana.support.service;

import com.arcane.arcana.common.entity.SupportRequest;
import com.arcane.arcana.support.dto.SupportDto;
import com.arcane.arcana.support.repository.SupportRepository;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.mail.javamail.JavaMailSender;
import org.springframework.mail.javamail.MimeMessageHelper;
import org.springframework.stereotype.Service;
import org.springframework.web.multipart.MultipartFile;

import jakarta.mail.internet.MimeMessage;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

@Service
public class SupportServiceImpl implements SupportService {

    private static final Logger logger = LoggerFactory.getLogger(SupportServiceImpl.class);
    private final SupportRepository supportRepository;
    private final JavaMailSender mailSender;

    @Value("${spring.mail.username}")
    private String senderEmail;

    public SupportServiceImpl(SupportRepository supportRepository, JavaMailSender mailSender) {
        this.supportRepository = supportRepository;
        this.mailSender = mailSender;
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
        sendSupportEmail(supportDto);
    }

    private void sendSupportEmail(SupportDto supportDto) {
        try {
            MimeMessage message = mailSender.createMimeMessage();
            MimeMessageHelper helper = new MimeMessageHelper(message, true, "UTF-8");

            helper.setFrom(senderEmail);
            helper.setTo(senderEmail);
            helper.setSubject("고객 문의: " + supportDto.getTitle());
            helper.setText("문의 유형: " + supportDto.getCategory() +
                "\n이메일: " + supportDto.getEmail() +
                "\n제목: " + supportDto.getTitle() +
                "\n설명: " + supportDto.getDescription());

            if (supportDto.getScreenshots() != null) {
                for (MultipartFile file : supportDto.getScreenshots()) {
                    if (file != null && !file.isEmpty()) {
                        helper.addAttachment(file.getOriginalFilename(), file);
                    }
                }
            }

            mailSender.send(message);
        } catch (Exception e) {
            logger.error("Failed to send support email: ", e);
            throw new RuntimeException("이메일 전송 실패", e);
        }
    }
}
